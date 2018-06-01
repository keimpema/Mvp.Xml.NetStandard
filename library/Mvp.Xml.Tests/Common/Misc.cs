using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;

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
	/// <summary>
	/// Miscelaneous tests.
	/// </summary>
	[TestClass]
	public class Misc
	{
		[TestMethod]
		public void SerializeXmlDocument()
		{
			XmlSerializer ser = new XmlSerializer(typeof(XmlDocument));
			int j = 3;
			double k = (double) j ;
			short s = 1;
			double ds = (double) s;

			Assert.IsNotNull(ser);
		}

		[TestMethod]
		public void CursorMovement()
		{
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.NorthwindResource));
			XPathNavigator nav = doc.CreateNavigator();

			nav.MoveToFirstChild();
			nav.MoveToFirstChild();

			XPathNavigator prev = nav.Clone();

			XPathNodeIterator it = nav.Select("//CustomerID");

			Assert.IsTrue(nav.IsSamePosition(prev));
		}
	}
}
