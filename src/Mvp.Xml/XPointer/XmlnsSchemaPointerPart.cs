using System;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Globalization;

namespace Mvp.Xml.XPointer
{
	/// <summary>
	/// xmlns() scheme based <see cref="XPointer"/> pointer part.
	/// </summary>
	internal class XmlnsSchemaPointerPart : PointerPart
	{
	    /// <summary>
		/// Creates xmlns() scheme pointer part with given
		/// namespace prefix and namespace URI. 
		/// </summary>
		/// <param name="prefix">Namespace prefix</param>
		/// <param name="uri">Namespace URI</param>
		public XmlnsSchemaPointerPart(string prefix, string uri)
		{
			Prefix = prefix;
			Uri = uri;
		}


	    public string Prefix { get; }

	    public string Uri { get; }


	    /// <summary>
		/// Evaluates <see cref="XPointer"/> pointer part and returns pointed nodes.
		/// </summary>
		/// <param name="doc">Document to evaluate pointer part on</param>
		/// <param name="nm">Namespace manager</param>
		/// <returns>Pointed nodes</returns>
		public override XPathNodeIterator Evaluate(XPathNavigator doc, XmlNamespaceManager nm)
		{
			nm.AddNamespace(Prefix, Uri);
			return null;
		}

	    public static XmlnsSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
		{
			//[1]   	XmlnsSchemeData	   ::=   	 NCName S? '=' S? EscapedNamespaceName
			//[2]   	EscapedNamespaceName	   ::=   	EscapedData*                      	                    
			//Read prefix as NCName
			lexer.NextLexeme();
			if (lexer.Kind != XPointerLexer.LexKind.NcName)
			{
				Debug.WriteLine(Properties.Resources.InvalidTokenInXmlnsSchemeWhileNCNameExpected);
				return null;
			}
			string prefix = lexer.NcName;
			lexer.SkipWhiteSpace();
			lexer.NextLexeme();
			if (lexer.Kind != XPointerLexer.LexKind.Eq)
			{				
				Debug.WriteLine(Properties.Resources.InvalidTokenInXmlnsSchemeWhileEqualsSignExpected);
				return null;
			}
			lexer.SkipWhiteSpace();
			string nsUri;
			try
			{
				nsUri = lexer.ParseEscapedData();
			}
			catch (Exception e)
			{
				throw new XPointerSyntaxException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SyntaxErrorInXmlnsSchemeData, e.Message));
			}
			return new XmlnsSchemaPointerPart(prefix, nsUri);
		}
	}
}
