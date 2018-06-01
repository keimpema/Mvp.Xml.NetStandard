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
    /// Collection of unit tests for GotDotNet Sets module functions.
    /// </summary>
    [TestClass]
    public class GDNSetsTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/GotDotNet/Sets/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/GotDotNet/Sets/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     set2:subset()
        /// </summary>
        [TestMethod]
        public void SubsetTest() 
        {
            RunAndCompare("source.xml", "subset.xslt", "subset.xml");
        }                
    }
}
