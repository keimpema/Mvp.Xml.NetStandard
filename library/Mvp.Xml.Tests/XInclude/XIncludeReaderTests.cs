using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Mvp.Xml.XInclude;
using Mvp.Xml.Common;

using System.Xml.Serialization;
using System.Net;
using System.Xml.Xsl;
using Mvp.Xml.Common.Xsl;
using System.Web;

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
    /// XIncludeReader general tests.
    /// </summary>
    [TestClass]
    public class XIncludeReaderTests
    {        

        public XIncludeReaderTests() 
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }
        
        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result) 
        {
            RunAndCompare(source, result, false);
        }        

        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result, bool textAsCDATA) 
        {
            RunAndCompare(source, result, textAsCDATA, null);
        }
        
        /// <summary>
        /// Utility method for running tests.
        /// </summary>        
        public static void RunAndCompare(string source, string result, bool textAsCDATA, XmlResolver resolver) 
        {                                 
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;                  
            XIncludingReader xir = new XIncludingReader(source);
            if (resolver != null)
                xir.XmlResolver = resolver;
            xir.ExposeTextInclusionsAsCDATA = textAsCDATA;            
//            while (xir.Read()) 
//            {
//                Console.WriteLine("{0} | {1} | {2} | {3}", xir.NodeType, xir.Name, xir.Value, xir.IsEmptyElement);                
//            }
//            throw new Exception();
            try 
            {
                doc.Load(xir);
            } 
            catch (Exception e)
            {
                xir.Close();
                throw e;
            }
            xir.Close();
            var s = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse ,
                IgnoreWhitespace = true
            };
            //s.ProhibitDtd = false;
            XmlReader r1 = XmlReader.Create(new StringReader(doc.OuterXml), s, doc.BaseURI);                        
            XmlReader r2 = XmlReader.Create(result, s);                        
            try 
            {
                while (r1.Read()) 
                {
                    Assert.IsTrue(r2.Read()); 
                    while (r1.NodeType == XmlNodeType.XmlDeclaration ||
                        r1.NodeType == XmlNodeType.Whitespace)
                        r1.Read();
                    while (r2.NodeType == XmlNodeType.XmlDeclaration ||
                        r2.NodeType == XmlNodeType.Whitespace)
                        r2.Read();
                    Assert.AreEqual(r1.XmlLang, r2.XmlLang);
                    switch (r1.NodeType) 
                    {
                        case XmlNodeType.Attribute:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Attribute);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.LocalName, r2.LocalName);
                            Assert.AreEqual(r1.NamespaceURI, r2.NamespaceURI);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;                   
                        case XmlNodeType.CDATA:
                            Assert.IsTrue(r2.NodeType == XmlNodeType.CDATA || r2.NodeType == XmlNodeType.Text);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Comment:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Comment);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.DocumentType:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.DocumentType);
                            Assert.AreEqual(r1.Name, r2.Name);                        
                            //Ok, don't compare DTD content
                            //Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Element:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Element);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.LocalName, r2.LocalName);
                            Assert.AreEqual(r1.NamespaceURI, r2.NamespaceURI);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Entity:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Entity);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.EndElement:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.EndElement);                        
                            break;
                        case XmlNodeType.EntityReference:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.EntityReference);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Notation:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.Notation);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.ProcessingInstruction);
                            Assert.AreEqual(r1.Name, r2.Name);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.SignificantWhitespace:
                            Assert.AreEqual(r2.NodeType, XmlNodeType.SignificantWhitespace);
                            Assert.AreEqual(r1.Value, r2.Value);
                            break;
                        case XmlNodeType.Text:
                            Assert.IsTrue(r2.NodeType == XmlNodeType.CDATA || r2.NodeType == XmlNodeType.Text);
                            Assert.AreEqual(r1.Value.Replace("\r\n", "\n").Replace("\r", "").Trim(), r2.Value.Replace("\r\n", "\n").Replace("\r", "").Trim());
                            break;                    
                        default:
                            break;
                    }                     
                }
                Assert.IsFalse(r2.Read());
                Assert.IsTrue(r1.ReadState == ReadState.EndOfFile || r1.ReadState == ReadState.Closed);
                Assert.IsTrue(r2.ReadState == ReadState.EndOfFile || r2.ReadState == ReadState.Closed);
            } 
            catch(Exception e) 
            {
                r1.Close();
                r1 = null;
                r2.Close();
                r2 = null;
                ReportResults(result, doc);
                throw e;
            }
            finally 
            {                
                if (r1 != null)
                    r1.Close();
                if (r2 != null)
                    r2.Close();
            }
            ReportResults(result, doc);
        }    
    
        private static void ReportResults(string expected, XmlDocument actual) 
        {
            StreamReader sr = new StreamReader(expected);
            string expectedResult = sr.ReadToEnd();
            sr.Close();                
            MemoryStream ms = new MemoryStream();
            actual.Save(new StreamWriter(ms, Encoding.UTF8));  
            ms.Position = 0;
            string actualResult = new StreamReader(ms).ReadToEnd();
            Console.WriteLine("\n-----------Expected result:-----------\n{0}", expectedResult);
            Console.WriteLine("-----------Actual result:-----------\n{0}", actualResult);
        }
      
        /// <summary>
        /// General test - it should work actually.
        /// </summary>
        [TestMethod]
        public void ItWorksAtLeast() 
        {
			RunAndCompare("../../../XInclude/tests/document.xml", "../../../XInclude/results/document.xml");            
        }
        

        /// <summary>
        /// Non XML character in the included document.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NonXmlCharacterException))]
        public void NonXMLChar() 
        {
			RunAndCompare("../../../XInclude/tests/nonxmlchar.xml", "../../../XInclude/results/nonxmlchar.xml");            
        }        

        /// <summary>
        /// File not found and no fallback.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FatalResourceException))]
        public void FileNotFound() 
        {
			RunAndCompare("../../../XInclude/tests/filenotfound.xml", "../../../XInclude/results/filenotfound.xml");            
        }        

        /// <summary>
        /// Includes itself by url.
        /// </summary>
        [TestMethod]        
        public void IncludesItselfByUrl() 
        {
			RunAndCompare("../../../XInclude/tests/includesitself.xml", "../../../XInclude/results/includesitself.xml");            
        }        

        /// <summary>
        /// Includes itself by url - no href - as text.
        /// </summary>
        [TestMethod]        
        [ExpectedException(typeof(FatalResourceException))]
        public void IncludesItselfNoHrefText() 
        {
			RunAndCompare("../../../XInclude/tests/includesitself-nohref-text.xml", "../../../XInclude/results/includesitself-nohref-text.xml");            
        }        

        /// <summary>
        /// Text inclusion. 
        /// </summary>
        [TestMethod]        
        public void TextInclusion() 
        {
			RunAndCompare("../../../XInclude/tests/working_example.xml", "../../../XInclude/results/working_example.xml");            
        }
        
        /// <summary>
        /// Text inclusion. 
        /// </summary>
        [TestMethod]        
        public void TextInclusion2() 
        {
			RunAndCompare("../../../XInclude/tests/working_example2.xml", "../../../XInclude/results/working_example2.xml");            
        }        

        /// <summary>
        /// Fallback.
        /// </summary>
        [TestMethod]        
        public void Fallback() 
        {
			RunAndCompare("../../../XInclude/tests/fallback.xml", "../../../XInclude/results/fallback.xml");            
        }        

        /// <summary>
        /// XPointer.
        /// </summary>
        [TestMethod]        
        public void XPointer() 
        {
			RunAndCompare("../../../XInclude/tests/xpointer.xml", "../../../XInclude/results/xpointer.xml");            
        }        

        /// <summary>
        /// xml:lang fixup
        /// </summary>
        [TestMethod]        
        public void XmlLangTest() 
        {
			RunAndCompare("../../../XInclude/tests/langtest.xml", "../../../XInclude/results/langtest.xml");                                    
        }        

        /// <summary>
        /// ReadOuterXml() test.
        /// </summary>
        [TestMethod]
        public void OuterXmlTest() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            xir.MoveToContent();
            string outerXml = xir.ReadOuterXml();
            xir.Close();
			xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            xir.MoveToContent();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xir);              
            string outerXml2 = doc.DocumentElement.OuterXml;
            Assert.AreEqual(outerXml, outerXml2);
        }        

        /// <summary>
        /// ReadInnerXml() test.
        /// </summary>
        [TestMethod]
        public void InnerXmlTest() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            xir.MoveToContent();
            string innerXml = xir.ReadInnerXml();
            xir.Close();
			xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            xir.MoveToContent();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(xir);            
            string innerXml2 = doc.DocumentElement.InnerXml;
            Assert.AreEqual(innerXml, innerXml2);
        }        

        /// <summary>
        /// Depth test.
        /// </summary>
        [TestMethod]
        public void DepthTest() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            StringBuilder sb = new StringBuilder();
            while (xir.Read()) 
            {
                Console.WriteLine("{0} | {1} | {2} | {3}", 
                    xir.NodeType, xir.Name, xir.Value, xir.Depth);
                sb.Append(xir.Depth);
            }
            string expected = "00011211111111223221100";
            Assert.AreEqual(sb.ToString(), expected);
        }
        
        /// <summary>
        /// Custom resolver test.
        /// </summary>
        [TestMethod]        
        public void CustomResolver() 
        {
			RunAndCompare("../../../XInclude/tests/resolver.xml", "../../../XInclude/results/resolver.xml", false, new TestResolver());            
        }

        /// <summary>
        /// Test for a bug discovered by Martin Wickett.
        /// </summary>
        [TestMethod]        
        public void Test_Martin() 
        {
			RunAndCompare("../../../XInclude/tests/test-Martin.xml", "../../../XInclude/results/test-Martin.xml");            
        }

        /// <summary>
        /// Test for string as input (no base URI)
        /// </summary>
        [TestMethod]        
        [ExpectedException(typeof(FatalResourceException))]
        public void NoBaseURITest() 
        {
			StreamReader sr = new StreamReader("../../../XInclude/tests/document.xml");
            string xml = sr.ReadToEnd();
            sr.Close();
            XIncludingReader xir = new XIncludingReader(new StringReader(xml));
            XmlWriter w = XmlWriter.Create(Console.Out);
            while (xir.Read());                
        }

        /// <summary>
        /// Caching test.
        /// </summary>
        [TestMethod]        
        public void CachingTest() 
        {
			RunAndCompare("../../../XInclude/tests/caching.xml", "../../../XInclude/results/caching.xml");            
        }

        /// <summary>
        /// Infinite loop (bug 1187498)
        /// </summary>
        [TestMethod]        
        public void LoopTest() 
        {
			RunAndCompare("../../../XInclude/tests/loop.xml", "../../../XInclude/results/loop.xml");            
        }

        [TestMethod]
        public void GetAttributeTest()
        {
            XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            while (xir.Read())
            {
                if (xir.NodeType == XmlNodeType.Element && xir.Name == "disclaimer")
                {
                    Assert.IsTrue(xir.AttributeCount == 1);
                    Assert.IsTrue(xir.GetAttribute(0).EndsWith("disclaimer.xml"));
                    Assert.IsTrue(xir[0].EndsWith("disclaimer.xml"));
                }
            }
        }

        [TestMethod]
        public void GetAttributeTest2()
        {
            XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document2.xml");
            xir.MakeRelativeBaseUri = false;
            while (xir.Read())
            {
                if (xir.NodeType == XmlNodeType.Element && xir.Name == "disclaimer")
                {
                    Assert.IsTrue(xir.AttributeCount == 1);                    
                    Assert.IsTrue(xir.GetAttribute(0).EndsWith("tests/disclaimerWithXmlBase.xml"));
                    Assert.IsTrue(xir[0].EndsWith("tests/disclaimerWithXmlBase.xml"));
                }
            }
        }

        [TestMethod]
        public void GetAttributeTest3()
        {
            XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document.xml");
            while (xir.Read())
            {
                if (xir.NodeType == XmlNodeType.Element && xir.Name == "disclaimer")
                {
                    Assert.IsTrue(xir.AttributeCount == 1);
                    xir.MoveToAttribute(0);
                    Assert.IsTrue(xir.Name == "xml:base");
                    Assert.IsTrue(xir.Value.EndsWith("disclaimer.xml"));
                }
            }
        }

        [TestMethod]
        public void GetAttributeTest4()
        {
            XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/document2.xml");
            xir.MakeRelativeBaseUri = false;
            while (xir.Read())
            {
                if (xir.NodeType == XmlNodeType.Element && xir.Name == "disclaimer")
                {
                    Assert.IsTrue(xir.AttributeCount == 1);
                    xir.MoveToAttribute(0);
                    Console.WriteLine(xir.Value);
                    Assert.IsTrue(xir.Name == "xml:base");
                    Assert.IsTrue(xir.Value.EndsWith("tests/disclaimerWithXmlBase.xml"));                                        
                }
            }
        }

        [TestMethod]
        public void SerializerTest()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Document));
            using (XIncludingReader r = new XIncludingReader("../../../XInclude/tests/Document3.xml"))
            {
                Document doc = (Document)ser.Deserialize(r);
                Assert.IsNotNull(doc);
                Assert.IsTrue(doc.Name == "Foo");
                Assert.IsNotNull(doc.Items);
                Assert.IsTrue(doc.Items.Length == 1);
                Assert.IsTrue(doc.Items[0].Value == "Bar");
            }            
        }

        [TestMethod]
        public void EncodingTest()
        {
            XmlTextReader r = new XmlTextReader(new StringReader("<foo/>"));
            XIncludingReader xir = new XIncludingReader(r);
            xir.ExposeTextInclusionsAsCDATA = true;
            xir.MoveToContent();
            Assert.IsTrue(xir.Encoding == UnicodeEncoding.Unicode);            
        }

        [TestMethod]
        public void ShouldPreserveCDATA()
        {
            string xml = "<HTML><![CDATA[<img src=\"/_layouts/images/\">]]></HTML>";
            XIncludingReader xir = new XIncludingReader(new StringReader(xml));
            xir.Read();
            Assert.AreEqual("<HTML><![CDATA[<img src=\"/_layouts/images/\">]]></HTML>", xir.ReadOuterXml());
        }

        [TestMethod]
        public void TestXPointerIndentationBug()
        {

            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;
            XsltSettings xsltSettings = new XsltSettings();
            xsltSettings.EnableDocumentFunction = true;
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            //settings.ProhibitDtd = false;
            XmlReader reader = XmlReader.Create("../../../XInclude/tests/Transform.xsl", settings);
            XIncludingReader xInputReader = new XIncludingReader("../../../XInclude/tests/FileA.xml");
            try
            {
                MvpXslTransform processor = new MvpXslTransform(false);
                processor.Load(reader, xsltSettings, resolver);                
                //xInputReader.XmlResolver = new XMLBase();
                XmlDocument xInputDoc = new XmlDocument();
                xInputDoc.Load(xInputReader);
                XmlInput xInput = new XmlInput(xInputDoc);
                StringWriter stringW = new StringWriter();
                XmlOutput xOutput = new XmlOutput(stringW);
                processor.Transform(xInput, null, xOutput);
                //processor.TemporaryFiles.Delete();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?>NodeA Content", stringW.ToString());
            }
            finally
            {
                 reader.Close();
                 xInputReader.Close();
            }
        }

        [TestMethod]
        public void TestLineInfo()
        {
            XIncludingReader r = new XIncludingReader("../../../XInclude/tests/document.xml");            
            IXmlLineInfo lineInfo = ((IXmlLineInfo)r);
            Assert.IsTrue(lineInfo.HasLineInfo());
            r.Read();
            Assert.AreEqual(1, lineInfo.LineNumber);
            Assert.AreEqual(3, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(1, lineInfo.LineNumber);
            Assert.AreEqual(22, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(2, lineInfo.LineNumber);
            Assert.AreEqual(2, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(2, lineInfo.LineNumber);
            Assert.AreEqual(54, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(6, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(8, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(54, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(56, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(1, lineInfo.LineNumber);
            Assert.AreEqual(22, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(2, lineInfo.LineNumber);
            Assert.AreEqual(5, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(2, lineInfo.LineNumber);
            Assert.AreEqual(17, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(3, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(3, lineInfo.LineNumber);
            Assert.AreEqual(12, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(4, lineInfo.LineNumber);
            Assert.AreEqual(2, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(4, lineInfo.LineNumber);
            Assert.AreEqual(13, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(5, lineInfo.LineNumber);
            Assert.AreEqual(4, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(5, lineInfo.LineNumber);
            Assert.AreEqual(6, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(7, lineInfo.LineNumber);
            Assert.AreEqual(18, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(7, lineInfo.LineNumber);
            Assert.AreEqual(20, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(8, lineInfo.LineNumber);
            Assert.AreEqual(3, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(4, lineInfo.LineNumber);
            Assert.AreEqual(75, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(6, lineInfo.LineNumber);
            Assert.AreEqual(3, lineInfo.LinePosition);
            r.Read();
            Assert.AreEqual(6, lineInfo.LineNumber);
            Assert.AreEqual(12, lineInfo.LinePosition);
        }
    }

    //public class XMLBase : XmlUrlResolver
    //{
    //    public override Uri ResolveUri(Uri baseUri, string relativeUri)
    //    {
    //        return base.ResolveUri(baseUri, HttpContext.Current.Server.MapPath("/") + relativeUri);
    //    }
    //}

    public class Document
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private Item[] items;

        public Item[] Items
        {
            get { return items; }
            set { items = value; }
        }
    }

    public class Item
    {
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }

    public class TestResolver : XmlUrlResolver 
    {
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "textreader")
                return new StringReader(@"<text attr=""val"">From custom resolver (as TextReader)</text>"); 
            else if (absoluteUri.Scheme == "stream") 
            {
				return File.OpenRead("../../../XInclude/results/document.xml");
            }
            else if (absoluteUri.Scheme == "xmlreader") 
            {                
                return XmlReader.Create(new StringReader(@"<text attr=""val"">From custom resolver (as XmlReader)</text>"), null, absoluteUri.AbsoluteUri); 
            }
            else
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

    }
}
