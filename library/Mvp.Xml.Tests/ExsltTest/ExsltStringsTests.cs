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
	/// Collection of unit tests for EXSLT Strings module functions.
	/// </summary>
	[TestClass]
	public class ExsltStringsTests : ExsltUnitTests
	{        
        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/EXSLT/Strings/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/Strings/"; }
        }                

        /// <summary>
        /// Tests the following function:
        ///     str:tokenize()
        /// </summary>
        [TestMethod]
        public void TokenizeTest() 
        {
            RunAndCompare("source.xml", "tokenize.xslt", "tokenize.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:replace()
        /// </summary>
        [TestMethod]
        public void ReplaceTest() 
        {
            RunAndCompare("source.xml", "replace.xslt", "replace.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:padding()
        /// </summary>
        [TestMethod]
        public void PaddingTest() 
        {
            RunAndCompare("source.xml", "padding.xslt", "padding.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:align()
        /// </summary>
        [TestMethod]
        public void AlignTest() 
        {
            RunAndCompare("source.xml", "align.xslt", "align.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:encode-uri()
        /// </summary>
        [TestMethod]
        public void EncodeUriTest() 
        {
            RunAndCompare("source.xml", "encode-uri.xslt", "encode-uri.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:decode-uri()
        /// </summary>
        [TestMethod]
        public void DecodeUriTest() 
        {
            RunAndCompare("source.xml", "decode-uri.xslt", "decode-uri.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:concat()
        /// </summary>
        [TestMethod]
        public void ConcatTest() 
        {
            RunAndCompare("source.xml", "concat.xslt", "concat.xml");
        }

        /// <summary>
        /// Tests the following function:
        ///     str:split()
        /// </summary>
        [TestMethod]
        public void SplitTest() 
        {
            RunAndCompare("source.xml", "split.xslt", "split.xml");
        }
	}
}
