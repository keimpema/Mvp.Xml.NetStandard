using System;
using System.Xml;
using System.Xml.XPath;

using Mvp.Xml.Common;
using Mvp.Xml.Common.XPath;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Mvp.Xml.Tests
{
	[TestClass]
	public class XPathCacheTests
	{
		private XPathDocument Document;
		private XPathDocument DocumentNoNs;

		[TestInitialize]
		public void Setup()
		{
			Document = new XPathDocument(Globals.GetResource(Globals.PubsNsResource));
			DocumentNoNs = new XPathDocument(Globals.GetResource(Globals.PubsResource));
		}

		[TestCleanup]
		public void TearDown()
		{
			Document = null;
		}

		[TestMethod]
		public void DefaultNamespace()
		{
			dsPubs pubs = new dsPubs();
			pubs.id = "0736";

			System.Xml.Serialization.XmlSerializer ser = 
				new System.Xml.Serialization.XmlSerializer(typeof(dsPubs), String.Empty);

			System.IO.StringWriter sw = new System.IO.StringWriter();
			ser.Serialize( sw, pubs );
            
			string xml = sw.ToString();
			Assert.IsNotNull(xml);
		}

		[TestMethod]
		public void DynamicVariable()
		{
			string expr = "//mvp:titles[mvp:price > 10]";
			string dyn = "//mvp:titles[mvp:price > $price]";
			int price = 10;

			XPathNavigator docnav = Document.CreateNavigator();
			XPathExpression xpath = docnav.Compile(expr);
			XmlNamespaceManager mgr = new XmlNamespaceManager(docnav.NameTable);
			mgr.AddNamespace(Globals.MvpPrefix, Globals.MvpNamespace);
			xpath.SetContext(mgr);

			int count1 = Document.CreateNavigator().Select(xpath).Count;
			// Test with compiled expression.
			int count2 = XPathCache.Select(expr, Document.CreateNavigator(), mgr).Count;

			Assert.AreEqual(count1, count2);

			// Test with dynamic expression.
			count2 = XPathCache.Select(dyn, Document.CreateNavigator(), 
				mgr, new XPathVariable("price", price)).Count;

			Assert.AreEqual(count1, count2);
		}

		[TestMethod]
		public void PrefixMapping()
		{
            string expr = "//mvp:titles[mvp:price > 10]";

			XPathNavigator docnav = Document.CreateNavigator();
			XPathExpression xpath = docnav.Compile(expr);
			XmlNamespaceManager mgr = new XmlNamespaceManager(docnav.NameTable);
			mgr.AddNamespace(Globals.MvpPrefix, Globals.MvpNamespace);
			xpath.SetContext(mgr);

			int count1 = Document.CreateNavigator().Select(xpath).Count;
			int count2 = XPathCache.Select(expr, Document.CreateNavigator(), 
				new XmlPrefix(Globals.MvpPrefix, Globals.MvpNamespace, Document.CreateNavigator().NameTable)).Count;
            
			Assert.AreEqual(count1, count2);
		}

		[TestMethod]
		public void Sorted1()
		{
			string expr = "//mvp:titles";

			XPathNavigator docnav = Document.CreateNavigator();
			XPathExpression xpath = docnav.Compile(expr);
			XmlNamespaceManager mgr = new XmlNamespaceManager(docnav.NameTable);
			mgr.AddNamespace(Globals.MvpPrefix, Globals.MvpNamespace);
			xpath.SetContext(mgr);
			XPathExpression sort = docnav.Compile("mvp:price");
			sort.SetContext(mgr);
			xpath.AddSort(sort, XmlSortOrder.Ascending, XmlCaseOrder.LowerFirst, String.Empty, XmlDataType.Number);

			XPathNodeIterator it = Document.CreateNavigator().Select(xpath);

			DebugUtils.XPathNodeIteratorToConsole(it);

			it = Document.CreateNavigator().Select(xpath);

			it.MoveNext();
			it.Current.MoveToFirstChild();
			string id1 = it.Current.Value;
			
			XPathNodeIterator cached = XPathCache.SelectSorted(
				expr, Document.CreateNavigator(),
				"mvp:price", XmlSortOrder.Ascending, XmlCaseOrder.LowerFirst, String.Empty, XmlDataType.Number, 
				new XmlPrefix(Globals.MvpPrefix, Globals.MvpNamespace, Document.CreateNavigator().NameTable));

			DebugUtils.XPathNodeIteratorToConsole(cached);

			cached = XPathCache.SelectSorted(
				expr, Document.CreateNavigator(),
				"mvp:price", XmlSortOrder.Ascending, XmlCaseOrder.LowerFirst, String.Empty, XmlDataType.Number, 
				new XmlPrefix(Globals.MvpPrefix, Globals.MvpNamespace, Document.CreateNavigator().NameTable));

			cached.MoveNext();
			cached.Current.MoveToFirstChild();
			string id2 = cached.Current.Value;

			Assert.AreEqual(id1, id2);
		}
	}
}
