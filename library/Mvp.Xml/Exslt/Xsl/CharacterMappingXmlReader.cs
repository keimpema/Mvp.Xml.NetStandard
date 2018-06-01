using System.Collections.Generic;
using System.Xml;

// ReSharper disable RedundantBaseQualifier
// ReSharper disable once CheckNamespace

namespace Mvp.Xml.Common.Xsl
{
    /// <summary>
    /// <see cref="XmlReader"/> implementation able to read and filter out XSLT 2.0-like character map declarations
    /// from XSLT stylesheets.
    /// For character mapping semantics see http://www.w3.org/TR/xslt20/#character-maps.
    /// The only deviation from XSLT 2.0 is that "output", "character-map" and "output-character" elements
    /// must be in the "http://www.xmllab.net/nxslt" namespace:
    /// <pre>
    /// &lt;xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    ///     xmlns:nxslt="http://www.xmllab.net/nxslt">
    ///    &lt;nxslt:output use-character-maps="testmap"/>
    ///    &lt;nxslt:character-map name="testmap">
    ///         &lt;nxslt:output-character character="&#160;" string="&amp;nbsp;" />
    ///    &lt;/nxslt:character-map>
    /// </pre>
    /// When reading is done, resulting compiled character map can be compiled calling 
    /// <see cref="CharacterMappingXmlReader.CompileCharacterMapping()"/> method.
    /// </summary>
    /// <remarks>
    /// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>
    public class CharacterMappingXmlReader : XmlWrappingReader 
    {
        private CharacterMapping mapping;
        private readonly string nxsltNamespace;
        private readonly string characterMapTag;
        private readonly string nameTag;
        private readonly string outputCharacterTag;
        private readonly string characterTag;
        private readonly string stringTag;
        private readonly string outputTag;
        private readonly string useCharacterMapsTag;
        private List<string> useCharacterMaps;
        private string currMapName;
        private CharacterMap currMap;

        /// <summary>
        /// Creates new instance of the <see cref="CharacterMappingXmlReader"/> with given
        /// base <see cref="XmlReader"/>.
        /// </summary>        
        public CharacterMappingXmlReader(XmlReader baseReader)
            : base(baseReader)
        {
            nxsltNamespace = base.NameTable.Add("http://www.xmllab.net/nxslt");
            characterMapTag = base.NameTable.Add("character-map");
            nameTag = base.NameTable.Add("name");
            outputCharacterTag = base.NameTable.Add("output-character");
            characterTag = base.NameTable.Add("character");
            stringTag = base.NameTable.Add("string");
            outputTag = base.NameTable.Add("output");
            useCharacterMapsTag = base.NameTable.Add("use-character-maps");
        }        
        
        /// <summary>
        /// See <see cref="XmlReader.Read"/>.
        /// </summary>        
        public override bool Read()
        {
            bool baseRead = base.Read();            
            if (NodeType == XmlNodeType.Element && NamespaceURI == nxsltNamespace && LocalName == characterMapTag)
            {
                //nxslt:character-map
                currMapName = base[nameTag];
                if (string.IsNullOrEmpty(currMapName))
                {
                    throw new System.Xml.Xsl.XsltCompileException("Required 'name' attribute of nxslt:character-map element is missing.");
                }
                currMap = new CharacterMap();
                string referencedMaps = base[useCharacterMapsTag];
                if (!string.IsNullOrEmpty(referencedMaps))
                {
                    currMap.ReferencedCharacterMaps = referencedMaps.Split(' ');
                }                                                                
            }
            else if (NodeType == XmlNodeType.EndElement && NamespaceURI == nxsltNamespace && LocalName == characterMapTag)
            {
                if (mapping == null)
                {
                    mapping = new CharacterMapping();
                }
                mapping.AddCharacterMap(currMapName, currMap);
            }
            else if (NodeType == XmlNodeType.Element && NamespaceURI == nxsltNamespace && LocalName == outputCharacterTag)
            {
                //nxslt:output-character                        
                string character = base[characterTag];
                if (string.IsNullOrEmpty(character))
                {
                    throw new System.Xml.Xsl.XsltCompileException("Required 'character' attribute of nxslt:output-character element is missing.");
                }
                if (character.Length > 1)
                {
                    throw new System.Xml.Xsl.XsltCompileException("'character' attribute value of nxslt:output-character element is too long - must be a single character.");
                }
                string _string = base[stringTag];
                if (string.IsNullOrEmpty(character))
                {
                    throw new System.Xml.Xsl.XsltCompileException("Required 'string' attribute of nxslt:output-character element is missing.");
                }
                currMap.AddMapping(character[0], _string);
            }
            else if (NodeType == XmlNodeType.Element && NamespaceURI == nxsltNamespace && LocalName == outputTag)
            {
                //nxslt:output
                string useMaps = base[useCharacterMapsTag];
                if (string.IsNullOrEmpty(useMaps))
                {
                    return baseRead;
                }

                if (useCharacterMaps == null)
                {
                    useCharacterMaps = new List<string>();
                }
                useCharacterMaps.AddRange(useMaps.Split(' '));
            }
            return baseRead;
        }

        /// <summary>
        /// Compiles character map.
        /// </summary>        
        public Dictionary<char, string> CompileCharacterMapping()
        {
            return mapping == null ? new Dictionary<char,string>() : mapping.Compile(useCharacterMaps);
        }
    }
}
