using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Common.Xsl {
    
    /// <summary>
    /// Represents XSLT 2.0 Character map, see http://www.w3.org/TR/xslt20/#character-maps.
    /// </summary>
    /// <remarks>
    /// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>
    internal class CharacterMap {
        /// <summary>
        /// Creates empty character map.
        /// </summary>
        public CharacterMap() {                        
            Map = new Dictionary<char, string>();
        }

        /// <summary>
        /// Adds mapping for given character.
        /// </summary>        
        public void AddMapping(char character, string replace)
        {
            if (Map.ContainsKey(character))
            {
                Map[character] = replace;
            }
            else
            {
                Map.Add(character, replace);
            }
        }                

        /// <summary>
        /// Gets mapping collection.
        /// </summary>
        public Dictionary<char, string> Map { get; }

        /// <summary>
        /// Referenced character maps.
        /// </summary>
        public string[] ReferencedCharacterMaps { get; set; }
    }
}
