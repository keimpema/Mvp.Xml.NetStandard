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
    /// Collection of unit tests for EXSLT Dates and Times module functions.
    /// </summary>
    [TestClass]
    public class ExsltDatesAndTimesTests : ExsltUnitTests
    {        
        protected override string TestDir 
        {
            get { return "../../../ExsltTest/tests/EXSLT/DatesAndTimes/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/DatesAndTimes/"; }
        }   
        
        /// <summary>
        /// Tests the following function:
        ///     date:format-date()
        /// </summary>
        [TestMethod]
        public void FormatDateTest() 
        {
            RunAndCompare("source.xml", "format-date.xslt", "format-date.xml");
        }                

        /// <summary>
        /// Tests the following function:
        ///     date:parse-date()
        /// </summary>
        [TestMethod]
        public void ParseDateTest() 
        {
            RunAndCompare("source.xml", "parse-date.xslt", "parse-date.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:date-time()
        /// </summary>
        [TestMethod]
        public void DateTimeTest() 
        {
            RunAndCompare("source.xml", "date-time.xslt", "date-time.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:date()
        /// </summary>
        [TestMethod]
        public void DateTest() 
        {
            RunAndCompare("source.xml", "date.xslt", "date.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:time()
        /// </summary>
        [TestMethod]
        public void TimeTest() 
        {
            RunAndCompare("source.xml", "time.xslt", "time.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:year()
        /// </summary>
        [TestMethod]
        public void YearTest() 
        {
            RunAndCompare("source.xml", "year.xslt", "year.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:leap-year()
        /// </summary>
        [TestMethod]
        public void LeapYearTest() 
        {
            RunAndCompare("source.xml", "leap-year.xslt", "leap-year.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:month-in-year()
        /// </summary>
        [TestMethod]
        public void MonthInYearTest() 
        {
            RunAndCompare("source.xml", "month-in-year.xslt", "month-in-year.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:month-name()
        /// </summary>
        [TestMethod]
        public void MonthNameTest() 
        {
            RunAndCompare("source.xml", "month-name.xslt", "month-name.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:month-abbreviation()
        /// </summary>
        [TestMethod]
        public void MonthAbbreviationTest() 
        {
            RunAndCompare("source.xml", "month-abbreviation.xslt", "month-abbreviation.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:week-in-year()
        /// </summary>
        [TestMethod]
        public void WeekInYearTest() 
        {
            RunAndCompare("source.xml", "week-in-year.xslt", "week-in-year.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:week-in-month()
        /// </summary>
        [TestMethod]
        public void WeekInMonthTest() 
        {
            RunAndCompare("source.xml", "week-in-month.xslt", "week-in-month.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:day-in-year()
        /// </summary>
        [TestMethod]
        public void DayInYearTest() 
        {
            RunAndCompare("source.xml", "day-in-year.xslt", "day-in-year.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:day-in-month()
        /// </summary>
        [TestMethod]
        public void DayInMonthTest() 
        {
            RunAndCompare("source.xml", "day-in-month.xslt", "day-in-month.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:week-of-week-in-month()
        /// </summary>
        [TestMethod]
        public void DayOfWeekInMonthTest() 
        {
            RunAndCompare("source.xml", "day-of-week-in-month.xslt", "day-of-week-in-month.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:day-in-week()
        /// </summary>
        [TestMethod]
        public void DayInWeekTest() 
        {
            RunAndCompare("source.xml", "day-in-week.xslt", "day-in-week.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:day-name()
        /// </summary>
        [TestMethod]
        public void DayNameTest() 
        {
            RunAndCompare("source.xml", "day-name.xslt", "day-name.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:day-abbreviation()
        /// </summary>
        [TestMethod]
        public void DayAbbreviationTest() 
        {
            RunAndCompare("source.xml", "day-abbreviation.xslt", "day-abbreviation.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:hour-in-day()
        /// </summary>
        [TestMethod]
        public void HourInDayTest() 
        {
            RunAndCompare("source.xml", "hour-in-day.xslt", "hour-in-day.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:minute-in-hour()
        /// </summary>
        [TestMethod]
        public void MinuteInHourTest() 
        {
            RunAndCompare("source.xml", "minute-in-hour.xslt", "minute-in-hour.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:second-in-minute()
        /// </summary>
        [TestMethod]
        public void SecondInMinuteTest() 
        {
            RunAndCompare("source.xml", "second-in-minute.xslt", "second-in-minute.xml");
        }
        
        /// <summary>
        /// Tests the following function:
        ///     date:difference()
        /// </summary>
        [TestMethod]
        public void DifferenceTest() 
        {
            RunAndCompare("source.xml", "difference.xslt", "difference.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:add()
        /// </summary>
        [TestMethod]
        public void AddTest() 
        {
            RunAndCompare("source.xml", "add.xslt", "add.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:add-duration()
        /// </summary>
        [TestMethod]
        public void AddDurationTest() 
        {
            RunAndCompare("source.xml", "add-duration.xslt", "add-duration.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:sum()
        /// </summary>
        [TestMethod]
        public void SumTest() 
        {
            RunAndCompare("source.xml", "sum.xslt", "sum.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:seconds()
        /// </summary>
        [TestMethod]
        public void SecondsTest() 
        {
            RunAndCompare("source.xml", "seconds.xslt", "seconds.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     date:duration()
        /// </summary>
        [TestMethod]
        public void DurationTest() 
        {
            RunAndCompare("source.xml", "duration.xslt", "duration.xml");
        }        
    }
}
