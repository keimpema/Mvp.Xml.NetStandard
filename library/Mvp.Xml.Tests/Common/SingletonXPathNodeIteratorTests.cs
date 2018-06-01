using System;
using System.IO;
using System.Xml.XPath;

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
    public class SingletonXPathNodeIteratorTests
    {
        [TestMethod]
        public void Test1()
        {
            XPathDocument doc = new XPathDocument(new StringReader("<foo/>"));
            XPathNavigator node = doc.CreateNavigator().SelectSingleNode("/*");
            SingletonXPathNodeIterator ni = new SingletonXPathNodeIterator(node);
            Assert.IsTrue(ni.MoveNext());
            Assert.IsTrue(ni.Current == node);
            Assert.IsFalse(ni.MoveNext());
        }        
    }
}
