using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Mvp.Xml.XInclude;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Mvp.Xml.XInclude.Test
{
	/// <summary>
	/// Edinburgh University test cases from the XInclude Test suite.
	/// </summary>
	[TestClass]
	public class RedhatTests
	{
		public RedhatTests()
		{
			//Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
		}

        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result) 
        {
            XIncludeReaderTests.RunAndCompare(
				"../../../XInclude/XInclude-Test-Suite/Imaq/test/XInclude/docs/" + source,
				"../../../XInclude/XInclude-Test-Suite/Imaq/test/XInclude/docs/" + result);
        }
        
		
        /// <summary>
        /// Simple test of including another XML document.        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_01() 
        {
            RunAndCompare("include.xml", "../../../result/XInclude/include.xml");            
        }  
  

		
        /// <summary>
        /// Test recursive inclusion.        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_02() 
        {
            RunAndCompare("recursive.xml", "../../../result/XInclude/recursive.xml");            
        }  
  

		
        /// <summary>
        /// Simple test of including a set of nodes from an XML document.        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_03() 
        {
            RunAndCompare("nodes.xml", "../../../result/XInclude/nodes.xml");            
        }  
  

		
        /// <summary>
        /// including another XML document with IDs        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_04() 
        {
            RunAndCompare("docids.xml", "../../../result/XInclude/docids.xml");            
        }  
  

		
        /// <summary>
        /// Simple test of including another text document        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_05() 
        {
            RunAndCompare("txtinclude.xml", "../../../result/XInclude/txtinclude.xml");            
        }  
  

		
        /// <summary>
        /// Simple test of a fallback on unavailable URI.        
        /// </summary>
        [TestMethod]
        public void imaq_include_xml_06() 
        {
            RunAndCompare("fallback.xml", "../../../result/XInclude/fallback.xml");            
        }  
  

	
	}
}
