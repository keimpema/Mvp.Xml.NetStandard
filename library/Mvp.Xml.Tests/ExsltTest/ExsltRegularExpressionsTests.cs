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
    /// Collection of unit tests for EXSLT RegularExpressions module functions.
    /// </summary>
    [TestClass]
    public class ExsltRegularExpressionsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/EXSLT/RegularExpressions/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/RegularExpressions/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     regexp:test()
        /// </summary>
        [TestMethod]
        public void TestTest() 
        {
            RunAndCompare("source.xml", "test.xslt", "test.xml");
        }   
         
        /// <summary>
        /// Tests the following function:
        ///     regexp:match()
        /// </summary>
        [TestMethod]
        public void MatchTest() 
        {
            RunAndCompare("source.xml", "match.xslt", "match.xml");
        }   

        /// <summary>
        /// Tests the following function:
        ///     regexp:replace()
        /// </summary>
        [TestMethod]
        public void ReplaceTest() 
        {
            RunAndCompare("source.xml", "replace.xslt", "replace.xml");
        }   
    }
}
