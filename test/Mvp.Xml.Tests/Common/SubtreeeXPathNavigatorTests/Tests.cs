#region using

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

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


#endregion

namespace Mvp.Xml.Tests.SubtreeeXPathNavigatorTests
{
	[TestClass]
	public class SubtreeTests
	{
		[Ignore]
		[TestMethod]
		public void SubtreeSpeed() 
		{
			XPathDocument xdoc = new XPathDocument(Globals.GetResource(Globals.LibraryResource));
			XPathNavigator nav = xdoc.CreateNavigator();
			XmlDocument doc = new XmlDocument();
			doc.Load(Globals.GetResource(Globals.LibraryResource));

            XslCompiledTransform xslt = new XslCompiledTransform();
			xslt.Load("../../Common/SubtreeeXPathNavigatorTests/print_root.xsl");

            Stopwatch stopWatch = new Stopwatch();

			// Warmup
			MemoryStream stmdom = new MemoryStream();
			XmlDocument wd = new XmlDocument(); 
			wd.LoadXml(doc.DocumentElement.FirstChild.OuterXml);
			xslt.Transform(wd, null, stmdom);
			MemoryStream stmxpath = new MemoryStream();
			nav.MoveToRoot();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();
			xslt.Transform(new SubtreeXPathNavigator(nav), null, stmxpath);
			nav = doc.CreateNavigator();

			int count = 10;
			float dom = 0;
			float xpath = 0;

			for (int i = 0; i < count; i++)
			{
				GC.Collect();
				System.Threading.Thread.Sleep(1000);

				stmdom = new MemoryStream();

                stopWatch.Start();

				// Create a new document for each child
				foreach (XmlNode testNode in doc.DocumentElement.ChildNodes)
				{
					XmlDocument tmpDoc = new XmlDocument(); 
					tmpDoc.LoadXml(testNode.OuterXml);

					// Transform the subset.
					xslt.Transform(tmpDoc, null, stmdom);
				}
                stopWatch.Stop();
                dom += stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
				GC.Collect();
				System.Threading.Thread.Sleep(1000);
				
				stmxpath = new MemoryStream();

				XPathExpression expr = nav.Compile("/library/book");

                stopWatch.Start();
				XPathNodeIterator books = nav.Select(expr);
				while (books.MoveNext())
				{
					xslt.Transform(new SubtreeXPathNavigator(books.Current), null, stmxpath);
				}
                stopWatch.Stop();
                xpath += stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
			}

			Console.WriteLine("XmlDocument transformation: {0}", dom / count);
			Console.WriteLine("SubtreeXPathNavigator transformation: {0}", xpath / count);

			stmdom.Position = 0;
			stmxpath.Position = 0;

			Console.WriteLine(new StreamReader(stmdom).ReadToEnd());
			Console.WriteLine(new string('*', 100));
			Console.WriteLine(new string('*', 100));
			Console.WriteLine(new StreamReader(stmxpath).ReadToEnd());
		}

		[TestMethod]
		public void ShouldBeOnRootNodeTypeOnCreation()
		{
			string xml = @"
	<root>
		<salutations>
			<salute>Hi there <name>kzu</name>.</salute>
			<salute>Bye there <name>vga</name>.</salute>
		</salutations>
		<other>
			Hi there without salutations.
		</other>
	</root>";

			XmlReaderSettings set = new XmlReaderSettings();
			set.IgnoreWhitespace = true;
			XPathDocument doc = new XPathDocument(XmlReader.Create(new StringReader(xml), set));
			XPathNavigator nav = doc.CreateNavigator();

			nav.MoveToFirstChild();
			nav.MoveToFirstChild();

			SubtreeXPathNavigator subtree = new SubtreeXPathNavigator(nav);

			Assert.AreEqual(XPathNodeType.Root, subtree.NodeType);
		}

		[TestMethod]
		public void ShouldNotMoveOutsideRootXPathDocument()
		{
			string xml = @"
	<root>
		<salutations>
			<salute>Hi there <name>kzu</name>.</salute>
			<salute>Bye there <name>vga</name>.</salute>
		</salutations>
		<other>
			Hi there without salutations.
		</other>
	</root>";

			XmlReaderSettings set = new XmlReaderSettings();
			set.IgnoreWhitespace = true;
			XPathDocument doc = new XPathDocument(XmlReader.Create(new StringReader(xml), set));
			XPathNavigator nav = doc.CreateNavigator();

			nav.MoveToFirstChild(); //root
			nav.MoveToFirstChild(); //salutations

			SubtreeXPathNavigator subtree = new SubtreeXPathNavigator(nav);
			subtree.MoveToFirstChild(); //salutations
			Assert.AreEqual("salutations", subtree.LocalName);
			subtree.MoveToFirstChild(); //salute
			subtree.MoveToRoot();

			Assert.AreEqual(XPathNodeType.Root, subtree.NodeType);
			subtree.MoveToFirstChild(); //salutations
			Assert.AreEqual("salutations", subtree.LocalName);
		}

		[TestMethod]
		public void ShouldNotMoveOutsideRootXmlDocument()
		{
			string xml = @"
	<root>
		<salutations>
			<salute>Hi there <name>kzu</name>.</salute>
			<salute>Bye there <name>vga</name>.</salute>
		</salutations>
		<other>
			Hi there without salutations.
		</other>
	</root>";

			XmlReaderSettings set = new XmlReaderSettings();
			set.IgnoreWhitespace = true;
			XmlDocument doc = new XmlDocument();
			doc.Load(XmlReader.Create(new StringReader(xml), set));
			XPathNavigator nav = doc.CreateNavigator();

			nav.MoveToFirstChild(); //root
			nav.MoveToFirstChild(); //salutations

			SubtreeXPathNavigator subtree = new SubtreeXPathNavigator(nav);
			subtree.MoveToFirstChild(); //salutations
			Assert.AreEqual("salutations", subtree.LocalName);
			subtree.MoveToFirstChild(); //salute
			subtree.MoveToRoot();

			Assert.AreEqual(XPathNodeType.Root, subtree.NodeType);
			subtree.MoveToFirstChild(); //salutations
			Assert.AreEqual("salutations", subtree.LocalName);
		}
	}
}
