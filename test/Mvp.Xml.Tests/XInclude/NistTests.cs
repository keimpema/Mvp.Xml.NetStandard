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
    public class NISTTests
    {
        public NISTTests()
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result) 
        {
            XIncludeReaderTests.RunAndCompare(
				"../../../XInclude/XInclude-Test-Suite/Nist/test/docs/" + source,
				"../../../XInclude/XInclude-Test-Suite/Nist/test/docs/" + result);
        }
        
		
        /// <summary>
        /// Test the inclusion of another XML document.        
        /// </summary>
        [TestMethod]
        public void Nist_include_01() 
        {
            RunAndCompare("nist-include-01.xml", "../../result/nist-include-01.xml");            
        }  
  

		
        /// <summary>
        /// Test that the encoding attribute in the Xinclude element has no effect when parse="xml".        
        /// </summary>
        [TestMethod]
        public void Nist_include_02() 
        {
            RunAndCompare("nist-include-02.xml", "../../result/nist-include-02.xml");            
        }  
  

		
        /// <summary>
        /// Test that values other than xml or text, in the parse attribute of the XInclude
        /// element, result in fatal errors.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_03() 
        {
            RunAndCompare("nist-include-03.xml", "");            
        }  
  

		
        /// <summary>
        /// Test of fallback element appearing as a child of an xinclude element.        
        /// </summary>
        [TestMethod]
        public void Nist_include_04() 
        {
            RunAndCompare("nist-include-04.xml", "../../result/nist-include-04.xml");            
        }  
  

		
        /// <summary>
        /// Test a fallback element not appearing as a direct
        /// child of an xinclude element. A fatal error should be generated.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_05() 
        {
            RunAndCompare("nist-include-05.xml", "");            
        }  
  

		
        /// <summary>
        /// Test a fallback when a resource error occurs.        
        /// </summary>
        [TestMethod]
        public void Nist_include_06() 
        {
            RunAndCompare("nist-include-06.xml", "../../result/nist-include-06.xml");            
        }  
  

		
        /// <summary>
        /// Test an empty fallback element. The xinclude element is
        /// removed from the results.        
        /// </summary>
        [TestMethod]
        public void Nist_include_07() 
        {
            RunAndCompare("nist-include-07.xml", "../../result/nist-include-07.xml");            
        }  
  

		
        /// <summary>
        /// Test of a fallback element missing from the
        /// include element. A resource error results in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void Nist_include_08() 
        {
            RunAndCompare("nist-include-08.xml", "");            
        }  
  
		
        /// <summary>
        /// Test unqualified attributes in the include element. They must be ignored.        
        /// </summary>
        [TestMethod]
        public void Nist_include_09() 
        {
            RunAndCompare("nist-include-09.xml", "../../result/nist-include-09.xml");            
        }  
  

		
        /// <summary>
        /// Test content other than the fallback, in the xinclude element.
        /// This content must be ignored.        
        /// </summary>
        [TestMethod]
        public void Nist_include_10() 
        {
            RunAndCompare("nist-include-10.xml", "../../result/nist-include-10.xml");            
        }  
  


		
        /// <summary>
        /// Test a resource containing non-well-formed XML. The inclusion results in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void Nist_include_11() 
        {
            RunAndCompare("nist-include-11.xml", "");            
        }  
  

		
        /// <summary>
        /// Test that is a fatal error for an include element to contain more than one fallback elements.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_12() 
        {
            RunAndCompare("nist-include-12.xml", "");            
        }  
  

		
        /// <summary>
        /// Test a fallback element containing markup when parse="text".        
        /// </summary>
        [TestMethod]
        public void Nist_include_13() 
        {
            RunAndCompare("nist-include-13.xml", "../../result/nist-include-13.xml");            
        }  
  

		
        /// <summary>
        /// Test a fallback element containing markup when parse="text".        
        /// </summary>
        [TestMethod]
        public void Nist_include_14() 
        {
            RunAndCompare("nist-include-14.xml", "../../result/nist-include-14.xml");            
        }  
  

		
        /// <summary>
        /// It is illegal for an include element to point to itself, when parse="xml".        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularInclusionException))]
        public void Nist_include_15() 
        {
            RunAndCompare("nist-include-15.xml", "");            
        }  
  

		
        /// <summary>
        /// Test a document type declaration information item child
        /// in the resource information set. the DTD should be excluded for inclusion in the source infoset.        
        /// </summary>
        [TestMethod]
        public void Nist_include_16() 
        {
            RunAndCompare("nist-include-16.xml", "../../result/nist-include-16.xml");            
        }  
  

		
        /// <summary>
        /// Test intra-document reference within include elements.        
        /// </summary>
        /// <remarks>INTRA-DOCUMENT REFERENCES ARE NOT SUPPORTED BY THE XIncludingReader.</remarks>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void Nist_include_17() 
        {
            RunAndCompare("nist-include-17.xml", "../../result/nist-include-17.xml");            
        }  
  

		
        /// <summary>
        /// Simple test of including a set of nodes from an XML document.        
        /// </summary>
        [TestMethod]
        public void Nist_include_18() 
        {
            RunAndCompare("nist-include-18.xml", "../../result/nist-include-18.xml");            
        }  
  
		
        /// <summary>
        /// Test the inclusion of a set of nodes from an XML document.        
        /// </summary>
        [TestMethod]
        public void Nist_include_19() 
        {
            RunAndCompare("nist-include-19.xml", "../../result/nist-include-19.xml");            
        }  
  

		
        /// <summary>
        /// Test an include location identifying a document information item with
        /// an xpointer locating the document root. In this case
        /// the set of top level include items is the children of acquired infoset's
        /// document information item, except for the document type information
        /// item.        
        /// </summary>
        [TestMethod]
        public void Nist_include_20() 
        {
            RunAndCompare("nist-include-20.xml", "../../result/nist-include-20.xml");            
        }  
  

		
        /// <summary>
        /// Including an XML document with an unparsed entity.        
        /// </summary>
        [TestMethod]
        public void Nist_include_21() 
        {
            RunAndCompare("nist-include-21.xml", "../../result/nist-include-21.xml");            
        }  
  

		
        /// <summary>
        /// Testing when the document (top level) element in the source infoset is an
        /// include element.        
        /// </summary>
        [TestMethod]
        public void Nist_include_22() 
        {
            RunAndCompare("nist-include-22.xml", "../../result/nist-include-22.xml");            
        }  
  

		
        /// <summary>
        /// Testing an include element in the document (top-level) element in the source doc.
        /// Test should fail because is including more than one element.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MalformedXInclusionResultError))]
        public void Nist_include_23() 
        {
            RunAndCompare("nist-include-23.xml", "");            
        }  
  

		
        /// <summary>
        /// Testing an include element in the document (top-level) element in the source doc.
        /// Test should fail because is including only a processing instruction.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MalformedXInclusionResultError))]
        public void Nist_include_24() 
        {
            RunAndCompare("nist-include-24.xml", "nist-include-21.xml");            
        }  
  

		
        /// <summary>
        /// Testing an include element in the document (top-level) element in the source doc.
        /// Test should fail because is including only a comment.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MalformedXInclusionResultError))]
        public void Nist_include_25() 
        {
            RunAndCompare("nist-include-25.xml", "nist-include-21.xml");            
        }  
  

		
        /// <summary>
        /// Test relative URI references in the included infoset.        
        /// </summary>
        [TestMethod]
        public void Nist_include_26() 
        {
            RunAndCompare("nist-include-26.xml", "../../result/nist-include-26.xml");            
        }  
  
		
		
        /// <summary>
        /// Test that the encoding attribute when parse="xml" does not translate the incoming document.        
        /// </summary>
        [TestMethod]
        public void Nist_include_27() 
        {
            RunAndCompare("nist-include-27.xml", "../../result/nist-include-27.xml");            
        }  
  

		
        /// <summary>
        /// including another XML document with IDs, using a shorthand pointer.        
        /// </summary>
        [TestMethod]
        public void Nist_include_28() 
        {
            RunAndCompare("nist-include-28.xml", "../../result/nist-include-28.xml");            
        }  
  

		
        /// <summary>
        /// including another XML document with IDs, using a shorthand pointer.        
        /// </summary>
        [TestMethod]
        public void Nist_include_29() 
        {
            RunAndCompare("nist-include-29.xml", "../../result/nist-include-29.xml");            
        }  
  

		
        /// <summary>
        /// Including another XML document with IDs, using a shorthand pointer.        
        /// </summary>
        [TestMethod]
        public void Nist_include_30() 
        {
            RunAndCompare("nist-include-30.xml", "../../result/nist-include-30.xml");            
        }  
  

		
        /// <summary>
        /// Including an XML document using an XPointer element scheme.        
        /// </summary>
        [TestMethod]
        public void Nist_include_31() 
        {
            RunAndCompare("nist-include-31.xml", "../../result/nist-include-31.xml");            
        }  
  

		
        /// <summary>
        /// Including an XML document using an XPointer element scheme.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void Nist_include_32() 
        {
            RunAndCompare("nist-include-32.xml", "");            
        }  
  

		
        /// <summary>
        /// Including an XML document using an XPointer element scheme.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void Nist_include_33() 
        {
            RunAndCompare("nist-include-33.xml", "");            
        }  
  

		
        /// <summary>
        /// Including another XML document with ids using XPointer element scheme.        
        /// </summary>
        [TestMethod]
        public void Nist_include_34() 
        {
            RunAndCompare("nist-include-34.xml", "../../result/nist-include-34.xml");            
        }  
  

		
        /// <summary>
        /// Including an XML document using an XPointer element scheme.        
        /// </summary>
        [TestMethod]
        public void Nist_include_35() 
        {
            RunAndCompare("nist-include-35.xml", "../../result/nist-include-35.xml");            
        }  
  

		
        /// <summary>
        /// Including an XML document using an XPointer element scheme.        
        /// </summary>
        [TestMethod]
        public void Nist_include_36() 
        {
            RunAndCompare("nist-include-36.xml", "../../result/nist-include-36.xml");            
        }  
  

		
        /// <summary>
        /// Including another XML document using XPointer Framework scheme-base pointer.
        /// If the processor does not support the scheme used in a pointer part, it skip that pointer part.        
        /// </summary>
        [TestMethod]
        public void Nist_include_37() 
        {
            RunAndCompare("nist-include-37.xml", "../../result/nist-include-37.xml");            
        }  
  

		
        /// <summary>
        /// Including another XML document using XPointer Framework.
        /// If the processor does not support the scheme used in a pointer part, it skip that pointer part.        
        /// </summary>
        [TestMethod]
        public void Nist_include_38() 
        {
            RunAndCompare("nist-include-38.xml", "../../result/nist-include-38.xml");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The comment should be ignored .        
        /// </summary>
        [TestMethod]
        public void nist_include_39() 
        {
            RunAndCompare("nist-include-39.xml", "../../result/nist-include-39.xml");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The element should be ignored .        
        /// </summary>
        [TestMethod]
        public void nist_include_40() 
        {
            RunAndCompare("nist-include-40.xml", "../../result/nist-include-40.xml");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// This test should result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void nist_include_41() 
        {
            RunAndCompare("nist-include-41.xml", "");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The xinclude element may contain a fallback element;
        /// other elements from the xinclude namespace result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_42() 
        {
            RunAndCompare("nist-include-42.xml", "");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The content must be one fallback. This test should result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_43() 
        {
            RunAndCompare("nist-include-43.xml", "");            
        }  
  

		
        /// <summary>
        /// Test a resource that contains not-well-formed XML.
        /// This test should result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void Nist_include_44() 
        {
            RunAndCompare("nist-include-44.xml", "");            
        }  
  

		
        /// <summary>
        /// Test a resource that contains not-well-formed XML.
        /// This test should result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XmlException))]
        public void Nist_include_45() 
        {
            RunAndCompare("nist-include-45.xml", "");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The xinclude element may contain a fallback element; other elements from the xinclude namespace result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_46() 
        {
            RunAndCompare("nist-include-46.xml", "");            
        }  
  

		
        /// <summary>
        /// Testing the content of the xinclude element.
        /// The xinclude element may contain a fallback element; other elements from the xinclude namespace result in a fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void Nist_include_47() 
        {
            RunAndCompare("nist-include-47.xml", "");            
        }  
  

		
        /// <summary>
        /// It is a fatal error to resolve an xpointer scheme on a document
        /// that contains unexpanded entity reference information items.        
        /// </summary>
        /// <remarks>No support for unexpanded entity references here.</remarks>
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Nist_include_48() 
        {
            RunAndCompare("nist-include-48.xml", "");            
        }  
  

		
        /// <summary>
        /// The unexpanded entity reference information items, if present in the source infoset,
        /// will appear in the result infoset.        
        /// </summary>
        [TestMethod]
        public void Nist_include_49() 
        {
            RunAndCompare("nist-include-49.xml", "../../result/nist-include-49.xml");            
        }  
  

		
        /// <summary>
        /// Test an include location identifying the document information item without an Xpointer,
        /// The set of top-level included items should be the children of the acquired inforset's document
        /// information item, except for the document type declaration information item.        
        /// </summary>
        [TestMethod]
        public void Nist_include_50() 
        {
            RunAndCompare("nist-include-50.xml", "../../result/nist-include-50.xml");            
        }  
  

		
        /// <summary>
        /// Test an include location having an XPointer identifying a comment.
        /// The set of top-level included items should consist of the information item corresponding
        /// to the comment node in the acquired infoset.        
        /// </summary>
        [TestMethod]
        public void Nist_include_51() 
        {
            RunAndCompare("nist-include-51.xml", "../../result/nist-include-51.xml");            
        }  
  

		
        /// <summary>
        /// Test an include location having an XPointer identifying a processing instruction.
        /// The set of top-level included items should consist of the information item corresponding
        /// to the processing instruction node in the acquired infoset.        
        /// </summary>
        [TestMethod]
        public void Nist_include_52() 
        {
            RunAndCompare("nist-include-52.xml", "../../result/nist-include-52.xml");            
        }  
  

		
        /// <summary>
        /// Test that an include location identifying an attribute node will result in a
        /// fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AttributeOrNamespaceInIncludeLocationError))]
        public void Nist_include_53() 
        {
            RunAndCompare("nist-include-53.xml", "");            
        }  
  

		
        /// <summary>
        /// Test that an include location identifying an attribute node will result in a
        /// fatal error.        
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AttributeOrNamespaceInIncludeLocationError))]
        public void Nist_include_54() 
        {
            RunAndCompare("nist-include-54.xml", "");            
        }  
  

		
        /// <summary>
        /// Including a duplicate unparsed entity.
        /// Test should ignore duplicate unparsed entity.        
        /// </summary>
        /// <remarks>No support for unparsed entities here.</remarks>
        [TestMethod]
        public void Nist_include_55() 
        {
            RunAndCompare("nist-include-55.xml", "../../result/nist-include-55.xml");            
        }  
  

		
        /// <summary>
        /// Including an unparsed entity with same name,
        /// but different sysid. Test should fail.        
        /// </summary>
        /// <remarks>No support for unparsed entities here.</remarks>
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Nist_include_56() 
        {
            RunAndCompare("nist-include-56.xml", "");            
        }  	
    }
}