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
    /// Collection of unit tests for GotDotNet Strings module functions.
    /// </summary>
    [TestClass]
    public class GDNStringsTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/GotDotNet/Strings/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/GotDotNet/Strings/"; }
        }                

        /// <summary>
        /// Tests the following function:
        ///     str2:lowercase()
        /// </summary>
        [TestMethod]
        public void LowercaseTest() 
        {
            RunAndCompare("source.xml", "lowercase.xslt", "lowercase.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str2:uppercase()
        /// </summary>
        [TestMethod]
        public void UppercaseTest() 
        {
            RunAndCompare("source.xml", "uppercase.xslt", "uppercase.xml");
        }
    }
}
