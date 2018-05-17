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
    /// Collection of unit tests for GotDotNet Dates and Times module functions.
    /// </summary>
    [TestClass]
    public class GDNDatesAndTimesTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/GotDotNet/DatesAndTimes/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/GotDotNet/DatesAndTimes/"; }
        }   
        
        /// <summary>
        /// Tests the following function:
        ///     date2:avg()
        /// </summary>
        [TestMethod]
        public void AvgTest() 
        {
            RunAndCompare("source.xml", "avg.xslt", "avg.xml");
        } 
        
        /// <summary>
        /// Tests the following function:
        ///     date2:min()
        /// </summary>
        [TestMethod]
        public void MinTest() 
        {
            RunAndCompare("source.xml", "min.xslt", "min.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:max()
        /// </summary>
        [TestMethod]
        public void MaxTest() 
        {
            RunAndCompare("source.xml", "max.xslt", "max.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:day-name()
        /// </summary>
        [TestMethod]
        public void DayNameTest() 
        {
            RunAndCompare("source.xml", "day-name.xslt", "day-name.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:day-abbreviation()
        /// </summary>
        [TestMethod]
        public void DayAbbreviationTest() 
        {
            RunAndCompare("source.xml", "day-abbreviation.xslt", "day-abbreviation.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:month-name()
        /// </summary>
        [TestMethod]
        public void MonthNameTest() 
        {
            RunAndCompare("source.xml", "month-name.xslt", "month-name.xml");
        } 

        /// <summary>
        /// Tests the following function:
        ///     date2:month-abbreviation()
        /// </summary>
        [TestMethod]
        public void MonthAbbreviationTest() 
        {
            RunAndCompare("source.xml", "month-abbreviation.xslt", "month-abbreviation.xml");
        } 
    }
}
