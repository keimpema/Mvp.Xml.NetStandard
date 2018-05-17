using System.Reflection;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// IXsltContextFunction wrapper around extension function.
	/// </summary>
	internal class ExsltContextFunction : IXsltContextFunction
	{
		private MethodInfo method;
		private XPathResultType[] argTypes;
		private object ownerObj;

		public ExsltContextFunction(MethodInfo mi, XPathResultType[] argTypes,
			object owner)
		{
			method = mi;
			this.argTypes = argTypes;
			ownerObj = owner;
		}

	    public int Minargs
		{
			get { return argTypes.Length; }
		}

		public int Maxargs
		{
			get { return argTypes.Length; }
		}

		public XPathResultType[] ArgTypes
		{
			get { return argTypes; }
		}

		public XPathResultType ReturnType
		{
			get { return ExsltContext.ConvertToXPathType(method.ReturnType); }
		}

		public object Invoke(XsltContext xsltContext, object[] args,
			XPathNavigator docContext)
		{
			return method.Invoke(ownerObj, args);
		}
	}
}

