using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements additional functions in the http://gotdotnet.com/exslt/regular-expressions namespace.
	/// </summary>
	public class GdnRegularExpressions
	{

		/// <summary>
		/// Implements the following function 
		///		node-set tokenize(string, string)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="regexp"></param>
		/// <returns>This function breaks the input string into a sequence of strings, 
		/// treating any substring that matches the regexp as a separator. 
		/// The separators themselves are not returned. 
		/// The matching strings are returned as a set of 'match' elements.</returns>
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
		public XPathNodeIterator Tokenize(string str, string regexp)
		{

			RegexOptions options = RegexOptions.ECMAScript;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<matches/>");

			Regex regex = new Regex(regexp, options);

			foreach (string match in regex.Split(str))
			{

				XmlElement elem = doc.CreateElement("match");
				elem.InnerText = match;
				doc.DocumentElement.AppendChild(elem);
			}

			return doc.CreateNavigator().Select("//match");
		}

	    public XPathNodeIterator tokenize(string str, string regexp) => Tokenize(str, regexp);

        /// <summary>
        /// Implements the following function 
        ///		node-set tokenize(string, string, string)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexp"></param>		
        /// <param name="flags"></param>
        /// <returns>This function breaks the input string into a sequence of strings, 
        /// treating any substring that matches the regexp as a separator. 
        /// The separators themselves are not returned. 
        /// The matching strings are returned as a set of 'match' elements.</returns>
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
        public XPathNodeIterator Tokenize(string str, string regexp, string flags)
		{

			RegexOptions options = RegexOptions.ECMAScript;

			if (flags.IndexOf("m") != -1)
			{
				options |= RegexOptions.Multiline;
			}

			if (flags.IndexOf("i") != -1)
			{
				options |= RegexOptions.IgnoreCase;
			}

			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<matches/>");

			Regex regex = new Regex(regexp, options);

			foreach (string match in regex.Split(str))
			{

				XmlElement elem = doc.CreateElement("match");
				elem.InnerText = match;
				doc.DocumentElement.AppendChild(elem);
			}

			return doc.CreateNavigator().Select("//match");
		}

	    public XPathNodeIterator tokenize(string str, string regexp, string flags) => Tokenize(str, regexp, flags);

	}
}