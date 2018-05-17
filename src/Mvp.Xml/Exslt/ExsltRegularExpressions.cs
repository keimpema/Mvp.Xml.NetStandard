using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements the EXSLT functions in the http://exslt.org/regular-expressions namespace.
	/// </summary>
	public class ExsltRegularExpressions
	{

		/// <summary>
		/// Implements the following function 
		///		boolean test(string, string)
		/// </summary>
		/// <param name="str"></param>
		/// <param name="regexp"></param>
		/// <returns></returns>
		public bool Test(string str, string regexp)
		{

			RegexOptions options = RegexOptions.ECMAScript;

			Regex regex = new Regex(regexp, options);
			return regex.IsMatch(str);
		}

	    public bool test(string str, string regexp) => Test(str, regexp);

        /// <summary>
        /// Implements the following function 
        ///		boolean test(string, string, string)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexp"></param>
        /// <param name="flags">One of 'i', 'g' or 'm'</param>
        /// <returns></returns>
        /// <remarks>Supports the string 'm' as a flag which indicates multiline mode</remarks>
        public bool Test(string str, string regexp, string flags)
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


			Regex regex = new Regex(regexp, options);
			return regex.IsMatch(str);
		}

	    public bool test(string str, string regexp, string flags) => Test(str, regexp, flags);

        /// <summary>
        /// Implements the following function 
        ///		node-set match(string, string)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexp"></param>
        /// <returns> The matching strings are returned as a set of 'match' elements.</returns>
        public XPathNodeIterator Match(string str, string regexp)
		{

			RegexOptions options = RegexOptions.ECMAScript;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<matches/>");

			Regex regex = new Regex(regexp, options);

			foreach (Group g in regex.Match(str).Groups)
			{

				XmlElement elem = doc.CreateElement("match");
				elem.InnerText = g.Value;
				doc.DocumentElement.AppendChild(elem);
			}

			return doc.CreateNavigator().Select("//match");
		}

	    public XPathNodeIterator match(string str, string regexp) => Match(str, regexp);

        /// <summary>
        /// Implements the following function 
        ///		node-set match(string, string, string)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="regexp"></param>
        /// <param name="flags"></param>
        /// <returns> The matching strings are returned as a set of 'match' elements.</returns>
        public XPathNodeIterator Match(string str, string regexp, string flags)
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

			if (flags.IndexOf("g") == -1)
			{
				foreach (Group g in regex.Match(str).Groups)
				{
					XmlElement elem = doc.CreateElement("match");
					elem.InnerText = g.Value;
					doc.DocumentElement.AppendChild(elem);
				}

			}
			else
			{

				foreach (Match m in regex.Matches(str))
				{

					XmlElement elem = doc.CreateElement("match");
					elem.InnerText = m.Value;
					doc.DocumentElement.AppendChild(elem);

				}//foreach(match m...)				
			}

			return doc.CreateNavigator().Select("//match");
		}

	    public XPathNodeIterator match(string str, string regexp, string flags) => Match(str, regexp, flags);

        /// <summary>
        /// Implements the following function 
        ///		string replace(string, string, string, string)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regexp"></param>
        /// <param name="flags"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public string Replace(string input, string regexp, string flags, string replacement)
		{

			RegexOptions options = RegexOptions.ECMAScript;

			if (flags.IndexOf("i") != -1)
			{
				options |= RegexOptions.IgnoreCase;
			}

			Regex regex = new Regex(regexp, options);

			if (flags.IndexOf("g") != -1)
			{
				return regex.Replace(input, replacement);
			}
			else
			{
				return regex.Replace(input, replacement, 1);
			}
		}

	    public string replace(string input, string regexp, string flags, string replacement) => Replace(input, regexp, flags, replacement);
	}
}