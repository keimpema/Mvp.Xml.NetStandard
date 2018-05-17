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
    /// Collection of unit tests for EXSLT Math module functions.
    /// </summary>
    [TestClass]
    public class ExsltMathTests : ExsltUnitTests
    {        

        protected override string TestDir 
        {
			get { return "../../../ExsltTest/tests/EXSLT/Math/"; }
        }
        protected override string ResultsDir 
        {
			get { return "../../../ExsltTest/results/EXSLT/Math/"; }
        }   
                       
        /// <summary>
        /// Tests the following function:
        ///     math:min()
        /// </summary>
        [TestMethod]
        public void MinTest() 
        {
            RunAndCompare("source.xml", "min.xslt", "min.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:max()
        /// </summary>
        [TestMethod]
        public void MaxTest() 
        {
            RunAndCompare("source.xml", "max.xslt", "max.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:highest()
        /// </summary>
        [TestMethod]
        public void HighestTest() 
        {
            RunAndCompare("source.xml", "highest.xslt", "highest.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:lowest()
        /// </summary>
        [TestMethod]
        public void LowestTest() 
        {
            RunAndCompare("source.xml", "lowest.xslt", "lowest.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:abs()
        /// </summary>
        [TestMethod]
        public void AbsTest() 
        {
            RunAndCompare("source.xml", "abs.xslt", "abs.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:sqrt()
        /// </summary>
        [TestMethod]
        public void SqrtTest() 
        {
            RunAndCompare("source.xml", "sqrt.xslt", "sqrt.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:power()
        /// </summary>
        [TestMethod]
        public void PowerTest() 
        {
            RunAndCompare("source.xml", "power.xslt", "power.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:constant()
        /// </summary>
        [TestMethod]
        public void ConstantTest() 
        {
            RunAndCompare("source.xml", "constant.xslt", "constant.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:log()
        /// </summary>
        [TestMethod]
        public void LogTest() 
        {
            RunAndCompare("source.xml", "log.xslt", "log.xml");
        }            
    
        /// <summary>
        /// Tests the following function:
        ///     math:random()
        /// </summary>
        [TestMethod]
        public void RandomTest() 
        {
            RunAndCompare("source.xml", "random.xslt", "random.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:sin()
        /// </summary>
        [TestMethod]
        public void SinTest() 
        {
            RunAndCompare("source.xml", "sin.xslt", "sin.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:cos()
        /// </summary>
        [TestMethod]
        public void CosTest() 
        {
            RunAndCompare("source.xml", "cos.xslt", "cos.xml");
        }            
        
        /// <summary>
        /// Tests the following function:
        ///     math:tan()
        /// </summary>
        [TestMethod]
        public void TanTest() 
        {
            RunAndCompare("source.xml", "tan.xslt", "tan.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:asin()
        /// </summary>
        [TestMethod]
        public void AsinTest() 
        {
            RunAndCompare("source.xml", "asin.xslt", "asin.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:acos()
        /// </summary>
        [TestMethod]
        public void AcosTest() 
        {
            RunAndCompare("source.xml", "acos.xslt", "acos.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:atan()
        /// </summary>
        [TestMethod]
        public void AtanTest() 
        {
            RunAndCompare("source.xml", "atan.xslt", "atan.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:atan2()
        /// </summary>
        [TestMethod]
        public void Atan2Test() 
        {
            RunAndCompare("source.xml", "atan2.xslt", "atan2.xml");
        }            

        /// <summary>
        /// Tests the following function:
        ///     math:exp()
        /// </summary>
        [TestMethod]
        public void ExpTest() 
        {
            RunAndCompare("source.xml", "exp.xslt", "exp.xml");
        }            
    }
}
