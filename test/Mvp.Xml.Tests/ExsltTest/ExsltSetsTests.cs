using System;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace ExsltTest
{
    /// <summary>
    /// Collection of unit tests for EXSLT Sets module functions.
    /// </summary>
    [TestClass]
    public class ExsltSetsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/EXSLT/Sets/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/Sets/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     set:difference()
        /// </summary>
        [TestMethod]
        public void DifferenceTest() 
        {
            RunAndCompare("source.xml", "difference.xslt", "difference.xml");
        }
        
        /// <summary>
        /// Tests the following function:
        ///     set:intersection()
        /// </summary>
        [TestMethod]
        public void IntersectionTest() 
        {
            RunAndCompare("source.xml", "intersection.xslt", "intersection.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:distinct()
        /// </summary>
        [TestMethod]
        public void DistinctTest() 
        {
            RunAndCompare("source.xml", "distinct.xslt", "distinct.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:has-same-node()
        /// </summary>
        [TestMethod]
        public void HasSameNodeTest() 
        {
            RunAndCompare("source.xml", "has-same-node.xslt", "has-same-node.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:leading()
        /// </summary>
        [TestMethod]
        public void LeadingTest() 
        {
            RunAndCompare("source.xml", "leading.xslt", "leading.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     set:trailing()
        /// </summary>
        [TestMethod]
        public void TrailingTest() 
        {
            RunAndCompare("source.xml", "trailing.xslt", "trailing.xml");
        }        
    }
}
