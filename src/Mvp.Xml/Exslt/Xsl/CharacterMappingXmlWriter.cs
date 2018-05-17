using System.Collections.Generic;
using System.Text;
using System.Xml;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Common.Xsl
{
    /// <summary>
    /// <see cref="XmlWriter"/> implementation able to substitute characters appearing in text or attribute nodes.
    /// For character mapping semantics see http://www.w3.org/TR/xslt20/#character-maps.
    /// </summary>
    /// <remarks>
    /// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>
    public class CharacterMappingXmlWriter : XmlWrappingWriter
    {
        private Dictionary<char, string> mapping;
        private readonly CharacterMappingXmlReader reader;

        /// <summary>
        /// Creates new instance of the <see cref="CharacterMappingXmlWriter"/>
        /// with given base <see cref="XmlWriter"/> and charcter mapping.
        /// </summary>        
        public CharacterMappingXmlWriter(XmlWriter baseWriter, Dictionary<char, string> mapping)
            : base(baseWriter)
        {
            this.mapping = mapping;
        }

        /// <summary>
        /// /// Creates new instance of the <see cref="CharacterMappingXmlWriter"/>
        /// with given base <see cref="XmlWriter"/> and <see cref="CharacterMappingXmlReader"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterMappingXmlReader"/> paramter is used to get character mapping
        /// information.
        /// </remarks>
        public CharacterMappingXmlWriter(CharacterMappingXmlReader reader, XmlWriter baseWriter)
            : base(baseWriter)
        {
            this.reader = reader;
        }

        /// <summary>
        /// See <see cref="XmlWriter.WriteString"/>.
        /// </summary>        
        public override void WriteString(string text)
        {
            if (mapping == null && reader != null)
            {
                mapping = reader.CompileCharacterMapping();
            }

            if (mapping != null && mapping.Count > 0)
            {
                var buf = new StringBuilder();
                foreach (char c in text)
                {
                    if (mapping.ContainsKey(c))
                    {
                        FlushBuffer(buf);
                        WriteRaw(mapping[c]);
                    }
                    else
                    {
                        buf.Append(c);
                    }
                }

                FlushBuffer(buf);
            }
            else
            {
                base.WriteString(text);
            }
        }

        private void FlushBuffer(StringBuilder buf)
        {
            if (buf.Length <= 0)
            {
                return;
            }

            base.WriteString(buf.ToString());
            buf.Length = 0;
        }
    }
}
