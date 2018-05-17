using System;
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
    public class EmptyXPathNodeIteratorTests
    {
        [TestMethod]
        public void Test1()
        {
            EmptyXPathNodeIterator ni = EmptyXPathNodeIterator.Instance;
            while (ni.MoveNext())
            {
                Assert.Fail("EmptyXPathNodeIterator must be empty");   
            }
        }

        [TestMethod]
        public void Test2()
        {
            EmptyXPathNodeIterator ni = EmptyXPathNodeIterator.Instance;
            Assert.IsTrue(ni.MoveNext() == false);
            Assert.IsTrue(ni.Count == 0);
            Assert.IsTrue(ni.Current == null);
            Assert.IsTrue(ni.CurrentPosition == 0);
        }
    }
}
