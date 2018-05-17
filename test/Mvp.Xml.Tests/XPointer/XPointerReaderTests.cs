using System;
using System.Xml.XPath;
using System.Xml;
using System.Text;
using System.IO;
using System.Net;
using Mvp.Xml.Tests;
using Mvp.Xml.XPointer;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Mvp.Xml.XPointer.Test
{
	/// <summary>
	/// Unit tests for XPointerReader class.
	/// </summary>
	[TestClass]
	public class XPointerReaderTests
	{
        private XmlReaderSettings readerSettings;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public void InitTest()
        {
            readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse ,
                IgnoreWhitespace = true,
                XmlResolver = new XmlUrlResolver
                {
                    Credentials = CredentialCache.DefaultCredentials
                }
            };
            //readerSettings.ProhibitDtd = false;
        }        

        /// <summary>
        /// xmlns() + xpath1() + namespaces works
        /// </summary>
        [TestMethod]
        public void XmlNsXPath1SchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpath1(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }

        /// <summary>
        /// xpath1() + namespaces doesn't work w/o xmlns()
        /// </summary>
        [TestMethod]    
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeWithoutXmlnsTest() 
        {
            string xptr = "xpath1(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }

        /// <summary>
        /// xpath1() that doesn't select a node w/o namespaces
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeNoSelectedNodeTest() 
        {
            string xptr = "xpath1(no-such-node/foo)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                while (xpr.Read()) { }
            }
        }
        
        /// <summary>
        /// xpath1() that returns scalar value, not a node
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPath1SchemeScalarResultTest() 
        {
            string xptr = "xpath1(2+2)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                while (xpr.Read()) { }
            }
        }

        /// <summary>
        /// xmlns() + xpointer() + namespaces works
        /// </summary>
        [TestMethod]
        public void XmlNsXPointerSchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }

        /// <summary>
        /// xpointer() + namespaces doesn't work w/o xmlns()
        /// </summary>
        [TestMethod]    
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeWithoutXmlnsTest() 
        {
            string xptr = "xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }

        /// <summary>
        /// xpointer() that doesn't select a node w/o namespaces
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeNoSelectedNodeTest() 
        {
            string xptr = "xpointer(no-such-node/foo)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                while (xpr.Read()) { }
            }
        }
        
        /// <summary>
        /// xpointer() that returns scalar value, not a node
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XPointerSchemeScalarResultTest() 
        {
            string xptr = "xpointer(2+2)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                while (xpr.Read()) { }
            }
        }

        /// <summary>
        /// superfluous xmlns() doesn't hurt
        /// </summary>
        [TestMethod]		
        public void SuperfluousXmlNsSchemeTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xpointer(dsPubs/publishers[pub_id='1389']/pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name>Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }

        /// <summary>
        /// xpointer() + xmlns() + namespaces doesn't work
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void XmlnsAfterTest() 
        {
            string xptr = "xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)xmlns(m=mvp-xml)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                while (xpr.Read()) { }
            }
        }
        
        /// <summary>
        /// namespace re3efinition doesn't hurt
        /// </summary>
        [TestMethod]
        public void NamespaceRedefinitionTest() 
        {
            string xptr = "xmlns(m=mvp-xml)xmlns(m=http://foo.com)xmlns(m=mvp-xml)xpointer(m:dsPubs/m:publishers[m:pub_id='1389']/m:pub_name)";
            using (XmlReader reader = XmlReader.Create(
                Globals.GetResource(Globals.PubsNsResource),
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                StringBuilder sb = new StringBuilder();
                while (xpr.Read())
                {
                    sb.Append(xpr.ReadOuterXml());
                }
                string expected = @"<pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name><pub_name xmlns=""mvp-xml"">Algodata Infosystems</pub_name>";
                Assert.AreEqual(sb.ToString(), expected);
            }
        }
                
        /// <summary>
        /// Shorthand pointer works
        /// </summary>
        [TestMethod]
        public void ShorthandTest() 
        {           
            string xptr = "o10535";            
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml",
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<Item orderID=""o10535""><OrderDate> 6/13/95</OrderDate><ShipAddress> Mataderos  2312</ShipAddress></Item>";
                while (xpr.Read())
                {                    
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means shorthand XPointer didn't work as expected.");
            }
        }

        /// <summary>
        /// Shorthand pointer works via stream
        /// </summary>
        [TestMethod]
        public void ShorthandViaStreamTest() 
        {           
            string xptr = "o10535";                        
            FileInfo file = new FileInfo("../../../XPointer/northwind.xml");
            using (FileStream fs = file.OpenRead()) 
            {                
                XPointerReader xpr = new XPointerReader(
                    XmlReader.Create(fs, readerSettings, file.FullName), xptr);
                string expected = @"<Item orderID=""o10535""><OrderDate> 6/13/95</OrderDate><ShipAddress> Mataderos  2312</ShipAddress></Item>";
                while (xpr.Read()) 
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }            
                throw new InvalidOperationException("This means shorthand XPointer didn't work as expected.");
            }
        }

        /// <summary>
        /// Shorthand pointer points to nothing
        /// </summary>
        [TestMethod]
         [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void ShorthandNotFoundTest() 
        {           
            string xptr = "no-such-id";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml",
                readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);                                
            }
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [TestMethod]
        public void ElementSchemeTest() 
        {           
            string xptr = "element(o10535)";            
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<Item orderID=""o10535""><OrderDate> 6/13/95</OrderDate><ShipAddress> Mataderos  2312</ShipAddress></Item>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [TestMethod]
        public void ElementSchemeTest2() 
        {           
            string xptr = "element(o10535/1)";            
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<OrderDate> 6/13/95</OrderDate>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }

        /// <summary>
        /// element() scheme pointer works
        /// </summary>
        [TestMethod]
        public void ElementSchemeTest3() 
        {           
            string xptr = "element(/1/1/2)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }        

        /// <summary>
        /// element() scheme pointer points to nothing
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NoSubresourcesIdentifiedException))]
        public void ElementSchemeNotFoundTest() 
        {           
            string xptr = "element(no-such-id)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);                
            }        
        }

        /// <summary>
        /// compound pointer
        /// </summary>
        [TestMethod]
        public void CompoundPointerTest() 
        {           
            string xptr = "xmlns(p=12345)xpath1(/no/such/node) xpointer(/and/such) element(/1/1/2) element(o10535/1)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }       

        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [TestMethod]
        public void UnknownSchemeTest() 
        {           
            string xptr = "dummy(foo) element(/1/1/2)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }      
 
        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [TestMethod]
        public void UnknownSchemeTest2() 
        {           
            string xptr = "foo:dummy(bar) element(/1/1/2)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }      

        /// <summary>
        /// Unknown scheme pointer
        /// </summary>
        [TestMethod]        
        public void UnknownSchemeTest3() 
        {           
            string xptr = "xmlns(foo=http://foo.com/schemas)foo:dummy(bar) element(/1/1/2)";
            using (XmlReader reader = XmlReader.Create(
                "../../../XPointer/northwind.xml", readerSettings))
            {
                XPointerReader xpr = new XPointerReader(reader, xptr);
                string expected = @"<CompanyName> Alfreds Futterkiste</CompanyName>";
                while (xpr.Read())
                {
                    Assert.AreEqual(xpr.ReadOuterXml(), expected);
                    return;
                }
                throw new InvalidOperationException("This means XPointer didn't work as expected.");
            }
        }      
        
        /// <summary>
        /// XSD-defined ID
        /// </summary>
        //[TestMethod]        
        //public void XSDDefnedIDTest() 
        //{           
        //    string xptr = "element(id1389/1)";                        
        //    XmlReader reader = XmlReader.Create("../../pubsNS.xml");
        //    XPointerReader xpr = new XPointerReader(reader, xptr, true);
        //    string expected = @"<pub_name>Algodata Infosystems</pub_name>";
        //    while (xpr.Read()) 
        //    {
        //        Assert.AreEqual(xpr.ReadOuterXml(), expected);
        //        return;
        //    }            
        //    throw new InvalidOperationException("This means XPointer didn't work as expected.");
        //}      
	}
}
