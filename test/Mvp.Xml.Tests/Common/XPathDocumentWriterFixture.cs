using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.XPath;
using System.IO;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Mvp.Xml.Tests.Common
{
	[TestClass]
	public class XPathDocumentWriterFixture
	{
		[TestMethod]
		public void ShouldCreateXmlWriterForDocument()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter();
			Assert.IsNotNull(writer);
		}

		[TestMethod]
		public void ShouldAllowEmptyBaseUri()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter(String.Empty);
			Assert.IsNotNull(writer);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void ShouldThrowWithNullBaseUri()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter((string)null);
		}

		[ExpectedException(typeof(XmlException))]
		[TestMethod]
		public void ShouldThrowIfNoRootWritten()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter();
			XPathDocument doc = writer.Close();
		}

		[TestMethod]
		public void ShouldAcceptStartElementRootOnly()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter();
			writer.WriteStartElement("Foo");
			XPathDocument doc = writer.Close();
		}

		[TestMethod]
		public void ShouldWriteRootElement()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter();
			writer.WriteElementString("hello", "world");
			XPathDocument doc = writer.Close();

			Assert.IsNotNull(doc);

			string xml = GetXml(doc);

			Assert.AreEqual("<hello>world</hello>", xml);
		}

		[TestMethod]
		public void ShouldUseBaseUriForDocument()
		{
			XPathDocumentWriter writer = new XPathDocumentWriter("kzu-uri");
			writer.WriteStartElement("Foo");
			XPathDocument doc = writer.Close();

			Assert.IsNotNull(doc);
			Assert.AreEqual("kzu-uri", doc.CreateNavigator().BaseURI);
		}

		[TestMethod]
		public void WriterIsDisposable()
		{
			XPathDocument doc;
			using (XPathDocumentWriter writer = new XPathDocumentWriter())
			{
				writer.WriteElementString("hello", "world");
				doc = writer.Close();
			}

			Assert.IsNotNull(doc);
		}

		private static string GetXml(XPathDocument doc)
		{
			XPathNavigator nav = doc.CreateNavigator();
			StringWriter sw = new StringWriter();
			XmlWriterSettings set = new XmlWriterSettings();
			set.OmitXmlDeclaration = true;
			using (XmlWriter w = XmlWriter.Create(sw, set))
			{
				nav.WriteSubtree(w);
			}

			return sw.ToString();
		}
	}
}
