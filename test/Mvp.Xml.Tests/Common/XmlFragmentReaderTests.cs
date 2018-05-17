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
using Mvp.Xml.Common;
using System.Xml;
using System.IO;

namespace Mvp.Xml.Tests.Common
{
	[TestClass]
	public class XmlFragmentReaderTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowsIfRootNameIsNull()
		{
			new XmlFragmentReader((string)null, XmlReader.Create(new StringReader("<item>foo</item>")));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ThrowsIfRootNameIsEmpty()
		{
			new XmlFragmentReader(String.Empty, XmlReader.Create(new StringReader("<item>foo</item>")));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowsIfRootNamespaceIsNull()
		{
			new XmlFragmentReader("Foo", null, XmlReader.Create(new StringReader("<item>foo</item>")));
		}

		[TestMethod]
		public void RootNamespaceCanBeEmpty()
		{
			new XmlFragmentReader("Foo", String.Empty, XmlReader.Create(new StringReader("<item>foo</item>")));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowsIfNameIsNull()
		{
			new XmlFragmentReader((XmlQualifiedName)null, XmlReader.Create(new StringReader("<item>foo</item>")));
		}

		[TestMethod]
		public void CanFakeRoot()
		{
			XmlQualifiedName qname = new XmlQualifiedName("foo", "mvp-xml");
			XmlReader reader = new XmlFragmentReader(qname, XmlReader.Create(new StringReader("<item id='1'>foo</item>")));

			Assert.AreEqual(ReadState.Initial, reader.ReadState);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(qname.Name, reader.LocalName);
			Assert.AreEqual(qname.Namespace, reader.NamespaceURI);
			Assert.IsFalse(reader.HasAttributes);
			Assert.IsTrue(reader.Read());
			Assert.AreEqual("item", reader.LocalName);
			reader.Skip();
			Assert.AreEqual(qname.Name, reader.LocalName);
			Assert.AreEqual(qname.Namespace, reader.NamespaceURI);
			Assert.IsFalse(reader.Read());
			Assert.AreEqual(ReadState.EndOfFile, reader.ReadState);
		}

		[TestMethod]
		public void RootNameMatchesFake()
		{
			XmlReader reader = new XmlFragmentReader("foo", XmlReader.Create(new StringReader("<item/>")));

			Assert.IsTrue(reader.Read());
			Assert.AreEqual("foo", reader.LocalName);
		}

		[TestMethod]
		public void RootNamespaceMatchesFake()
		{
			XmlReader reader = new XmlFragmentReader("foo", "mvp-xml", XmlReader.Create(new StringReader("<item/>")));

			Assert.IsTrue(reader.Read());
			Assert.AreEqual("mvp-xml", reader.NamespaceURI);
		}

		[TestMethod]
		public void NamespaceURIRestoredAfterFake()
		{
			XmlReader reader = new XmlFragmentReader("foo", "mvp-xml", XmlReader.Create(new StringReader("<item/>")));

			Assert.IsTrue(reader.Read());
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(String.Empty, reader.NamespaceURI);
		}

		[TestMethod]
		public void CannotReadPastFake()
		{
			XmlReader reader = new XmlFragmentReader("foo", "mvp-xml", XmlReader.Create(new StringReader("<item/>")));

			Assert.IsTrue(reader.Read());
			Assert.IsTrue(reader.Read());
			Assert.IsTrue(reader.Read());
			Assert.AreEqual("mvp-xml", reader.NamespaceURI);
			Assert.IsFalse(reader.Read());
		}
	}
}
