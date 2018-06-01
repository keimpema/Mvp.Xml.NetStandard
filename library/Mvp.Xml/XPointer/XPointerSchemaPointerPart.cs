using System;
using System.Xml;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;
using System.Globalization;

namespace Mvp.Xml.XPointer
{
	/// <summary>
	/// xpointer() scheme based XPointer pointer part.
	/// </summary>
	internal class XPointerSchemaPointerPart : PointerPart
	{
	    private readonly string xpath;

	    public XPointerSchemaPointerPart(string xpath)
		{
			this.xpath = xpath;
		}

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

	    public static XPointerSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
		{
			try
			{
				return new XPointerSchemaPointerPart(lexer.ParseEscapedData());
			}
			catch (Exception e)
			{
				throw new XPointerSyntaxException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SyntaxErrorInXPointerSchemeData, e.Message));
			}
		}
	}
}
