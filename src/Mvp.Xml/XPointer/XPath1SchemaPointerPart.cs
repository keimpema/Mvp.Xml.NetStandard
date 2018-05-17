using System;
using System.Xml;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;
using System.Globalization;

namespace Mvp.Xml.XPointer
{
	/// <summary>
	/// xpath1() scheme based XPointer pointer part.
	/// </summary>
	internal class XPath1SchemaPointerPart : PointerPart
	{
	    private string xpath;

	    /// <summary>
		/// Evaluates <see cref="XPointer"/> pointer part and returns pointed nodes.
		/// </summary>
		/// <param name="doc">Document to evaluate pointer part on</param>
		/// <param name="nm">Namespace manager</param>
		/// <returns>Pointed nodes</returns>		    
		public override XPathNodeIterator Evaluate(XPathNavigator doc, XmlNamespaceManager nm)
		{
			try
			{
				return XPathCache.Select(xpath, doc, nm);
			}
			catch
			{
				return null;
			}
		}

	    public static XPath1SchemaPointerPart ParseSchemaData(XPointerLexer lexer)
		{
			XPath1SchemaPointerPart part = new XPath1SchemaPointerPart();
			try
			{
				part.xpath = lexer.ParseEscapedData();
			}
			catch (Exception e)
			{
				throw new XPointerSyntaxException(string.Format(
					CultureInfo.CurrentCulture,
					Properties.Resources.SyntaxErrorInXPath1SchemeData,
					e.Message));
			}
			return part;
		}
	}
}
