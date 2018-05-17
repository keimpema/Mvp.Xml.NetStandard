using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using Mvp.Xml.Common;

namespace Mvp.Xml.Tests.Common
{
    [TestClass]
    public class XhtmlWriterTests
    {
        private const string XHTML_NAMESPACE = "http://www.w3.org/1999/xhtml";

        [TestMethod]
        public void testShouldWriteEmptEndTags()
        {
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw);
            XhtmlWriter w = new XhtmlWriter(xw);
            w.WriteStartDocument();
            w.WriteStartElement("html", XHTML_NAMESPACE);
            writeElement(w, "area");
            writeElement(w, "base");
            writeElement(w, "basefont");
            writeElement(w, "br");
            writeElement(w, "col");
            writeElement(w, "frame");
            writeElement(w, "hr");
            writeElement(w, "img");
            writeElement(w, "input");
            writeElement(w, "isindex");
            writeElement(w, "link");
            writeElement(w, "meta");
            writeElement(w, "param");
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><html xmlns=\"http://www.w3.org/1999/xhtml\"><area /><base /><basefont /><br /><col /><frame /><hr /><img /><input /><isindex /><link /><meta /><param /></html>",
                sw.ToString());
        }

        [TestMethod]
        public void testShouldWriteFullEndTags()
        {
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw);
            XhtmlWriter w = new XhtmlWriter(xw);
            w.WriteStartDocument();
            w.WriteStartElement("html", XHTML_NAMESPACE);
            writeElement(w, "script");
            writeElement(w, "p");
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><html xmlns=\"http://www.w3.org/1999/xhtml\"><script></script><p></p></html>",
                sw.ToString());
        }

        [TestMethod]
        public void testShouldWriteEmptEndTagsEvenWithAttrs()
        {
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw);
            XhtmlWriter w = new XhtmlWriter(xw);
            w.WriteStartDocument();
            w.WriteStartElement("html", XHTML_NAMESPACE);
            writeElementWithAttrs(w, "area");
            writeElementWithAttrs(w, "base");
            writeElementWithAttrs(w, "basefont");
            writeElementWithAttrs(w, "br");            
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><html xmlns=\"http://www.w3.org/1999/xhtml\"><area foo=\"bar\" /><base foo=\"bar\" /><basefont foo=\"bar\" /><br foo=\"bar\" /></html>",
                sw.ToString());
        }
        
        private static void writeElementWithAttrs(XhtmlWriter w, string name)
        {
            w.WriteStartElement(name, XHTML_NAMESPACE);
            w.WriteAttributeString("foo", "bar");
            w.WriteEndElement();
        }
        
        private static void writeElement(XhtmlWriter w, string name)
        {
            w.WriteStartElement(name, XHTML_NAMESPACE);
            w.WriteEndElement();
        }
    }
}
