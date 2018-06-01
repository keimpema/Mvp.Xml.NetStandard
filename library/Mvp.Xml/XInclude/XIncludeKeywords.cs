using System.Xml;


namespace Mvp.Xml.XInclude
{
	/// <summary>
	/// XInclude syntax keyword collection.	
	/// </summary>
	/// <author>Oleg Tkachenko, http://www.xmllab.net</author>
	internal class XIncludeKeywords
	{
	    public XIncludeKeywords(XmlNameTable nt)
		{
			nameTable = nt;
			//Preload some keywords
			XIncludeNamespace = nameTable.Add(sXIncludeNamespace);
			OldXIncludeNamespace = nameTable.Add(sOldXIncludeNamespace);
			Include = nameTable.Add(sInclude);
			Href = nameTable.Add(sHref);
			Parse = nameTable.Add(sParse);
		}

	    //
		// Keyword strings
		private const string sXIncludeNamespace = "http://www.w3.org/2001/XInclude";
		private const string sOldXIncludeNamespace = "http://www.w3.org/2003/XInclude";
		private const string sInclude = "include";
		private const string sHref = "href";
		private const string sParse = "parse";
		private const string sXml = "xml";
		private const string sText = "text";
		private const string sXpointer = "xpointer";
		private const string sAccept = "accept";
		private const string sAcceptLanguage = "accept-language";
		private const string sEncoding = "encoding";
		private const string sFallback = "fallback";
		private const string sXmlNamespace = "http://www.w3.org/XML/1998/namespace";
		private const string sBase = "base";
		private const string sXmlBase = "xml:base";
		private const string sLang = "lang";
		private const string sXmlLang = "xml:lang";

	    private XmlNameTable nameTable;

		//
		// Properties
	    private string xml;
		private string text;
		private string xpointer;
		private string accept;
		private string acceptLanguage;
		private string encoding;
		private string fallback;
		private string xmlNamespace;
		private string _Base;
		private string xmlBase;
		private string lang;
		private string xmlLang;

	    // http://www.w3.org/2003/XInclude
		public string XIncludeNamespace { get; }

	    // http://www.w3.org/2001/XInclude
		public string OldXIncludeNamespace { get; }

	    // include
		public string Include { get; }

	    // href
		public string Href { get; }

	    // parse
		public string Parse { get; }

	    // xml
		public string Xml => xml ?? (xml = nameTable.Add(sXml));

	    // text
		public string Text => text ?? (text = nameTable.Add(sText));

	    // xpointer
		public string Xpointer => xpointer ?? (xpointer = nameTable.Add(sXpointer));

	    // accept
		public string Accept => accept ?? (accept = nameTable.Add(sAccept));

	    // accept-language
		public string AcceptLanguage => acceptLanguage ?? (acceptLanguage = nameTable.Add(sAcceptLanguage));

	    // encoding
		public string Encoding => encoding ?? (encoding = nameTable.Add(sEncoding));

	    // fallback
		public string Fallback => fallback ?? (fallback = nameTable.Add(sFallback));

	    // Xml namespace
		public string XmlNamespace => xmlNamespace ?? (xmlNamespace = nameTable.Add(sXmlNamespace));

	    // Base
		public string Base => _Base ?? (_Base = nameTable.Add(sBase));

	    // xml:base
		public string XmlBase => xmlBase ?? (xmlBase = nameTable.Add(sXmlBase));

	    // Lang
		public string Lang => lang ?? (lang = nameTable.Add(sLang));

	    // xml:lang
		public string XmlLang => xmlLang ?? (xmlLang = nameTable.Add(sXmlLang));

	    // Comparison
		public static bool Equals(string keyword1, string keyword2)
		{
		    return ReferenceEquals(keyword1, keyword2);
			//return (object)keyword1 == (object)keyword2;
		}
	}
}
