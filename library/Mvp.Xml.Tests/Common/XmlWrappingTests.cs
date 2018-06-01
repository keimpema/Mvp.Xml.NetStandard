#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mvp.Xml.Common;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mvp.Xml.Tests
{
	[TestClass]
	public class XmlWrappingTests
	{
		[TestMethod]
		public void ShouldCloseFullElement()
		{
			string xslt = @"
<xslt:stylesheet version=""1.0"" xmlns:xslt=""http://www.w3.org/1999/XSL/Transform"">
  <xslt:output indent=""yes""/>
  <xslt:variable name=""g_jsserver"">http://clariusconsulting.net/</xslt:variable>
  <xslt:template match=""/"">
	<page>
		<xslt:call-template name=""WriteScriptTag"">
		  <xslt:with-param name=""js_url"">foo.js</xslt:with-param>
		</xslt:call-template>
	</page>
  </xslt:template>
  <xslt:template name=""WriteScriptTag"">
    <xslt:param name=""js_url"" />
    <xslt:param name=""js_base"" select=""$g_jsserver"" />
    <xslt:if test=""$js_url!=''"">
      <script type=""text/javascript"">
        <xslt:attribute name=""src"">
          <xslt:choose>
            <xslt:when test=""substring($js_url,1,5) = 'http:'"">
              <xslt:value-of select=""$js_url"" />
            </xslt:when>
            <xslt:when test=""substring($js_url,1,6) = 'https:'"">
              <xslt:value-of select=""$js_url"" />
            </xslt:when>
            <xslt:otherwise>
              <xslt:value-of select=""$js_base"" />
              <xslt:value-of select=""$js_url"" />
            </xslt:otherwise>
          </xslt:choose>
        </xslt:attribute>
		<xslt:text> </xslt:text>
      </script>
    </xslt:if>
  </xslt:template>
</xslt:stylesheet>";

			string input = "<root />";

			StringWriter sw = new StringWriter();
			ScriptCloseWriter xw = new ScriptCloseWriter(XmlWriter.Create(sw));

			//Old v.1.x transform would also work fine.
			//XslTransform tx = new XslTransform();
			XslCompiledTransform tx = new XslCompiledTransform();
			tx.Load(XmlReader.Create(new StringReader(xslt)));

			tx.Transform(XmlReader.Create(new StringReader(input)), xw);

			xw.Close();

			Console.WriteLine(sw.ToString());
		}

		class ScriptCloseWriter : XmlWrappingWriter
		{
			string lastElement = String.Empty;

			public ScriptCloseWriter(XmlWriter baseWriter) : base(baseWriter) { }

			public override void WriteStartElement(string prefix, string localName, string ns)
			{
				base.WriteStartElement(prefix, localName, ns);
				lastElement = localName;
			}

			public override void WriteEndElement()
			{
				if (lastElement == "script")
				{
					base.WriteFullEndElement();
				}
				else
				{
					base.WriteEndElement();
				}
			}
		}
	}
}
