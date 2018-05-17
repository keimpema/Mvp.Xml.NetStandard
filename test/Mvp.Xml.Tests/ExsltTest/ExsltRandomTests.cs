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
    /// Collection of unit tests for EXSLT Random module functions.
    /// </summary>
    [TestClass]
    public class ExsltRandomTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/EXSLT/Random/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/Random/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     random:random-sequence()
        /// </summary>
        [TestMethod]
        public void RandomSequenceTest() 
        {
            RunAndCompare("source.xml", "random-sequence.xslt", "random-sequence.xml");
        }          
    }
}
