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

using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using Mvp.Xml.Common.Xsl;

namespace Mvp.Xml.Tests
{
    [TestClass]
    public class MvpXslTransformTests 
    {
        byte[] standardResult;
        byte[] resolverTestStandardResult;
        MvpXslTransform xslt, xslt2;
        XsltArgumentList args;

        public MvpXslTransformTests()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load("../../../Common/MvpXslTransformTests/test1.xslt");
            MemoryStream ms = new MemoryStream();
            using (XmlReader r = GetReader(Globals.NorthwindResource))
            {                
                xslt.Transform(r, Arguments, ms);
            }
            standardResult = ms.ToArray();
            XslCompiledTransform xslt2 = new XslCompiledTransform();
			xslt2.Load("../../../Common/MvpXslTransformTests/resolver-test.xslt", XsltSettings.TrustedXslt, null);
            MemoryStream ms2 = new MemoryStream();
            XmlWriter w = XmlWriter.Create(ms2);
			using (XmlReader r2 = XmlReader.Create("../../../Common/MvpXslTransformTests/test.xml"))
            {
                xslt2.Transform(r2, Arguments, w, new MyXmlResolver());
            }
            w.Close();
            resolverTestStandardResult = ms2.ToArray();              
        }

        private XsltArgumentList Arguments
        {
            get {
                if (args == null)
                {
                    args = new XsltArgumentList();
                    args.AddParam("prm1", "", "value1");
                }
                return args;
            }
        }

        private MvpXslTransform GetMvpXslTransform()
        {
            if (xslt == null)
            {
                xslt = new MvpXslTransform();
				xslt.Load("../../../Common/MvpXslTransformTests/test1.xslt");
            }
            return xslt;
        }

        private MvpXslTransform GetMvpXslTransform2()
        {
            if (xslt2 == null)
            {
                xslt2 = new MvpXslTransform();
				xslt2.Load("../../../Common/MvpXslTransformTests/resolver-test.xslt", XsltSettings.TrustedXslt, null);
            }
            return xslt2;
        }        

        private static void CompareResults(byte[] standard, byte[] test)
        {
            Assert.AreEqual(standard.Length, test.Length, string.Format("Lengths are different: {0}, {1}", standard.Length, test.Length));
            for (int i = 0; i < standard.Length; i++)
            {
                Assert.IsTrue(standard[i] == test[i], string.Format("Values aren't equal: {0}, {1}, positoin {2}", standard[i], test[i], i));
            }
        }

        private static XmlReader GetReader(string xml)
        {
            var s = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
            //s.ProhibitDtd = false;
            return XmlReader.Create(Globals.GetResource(xml), s);
        }

        [TestMethod]
        public void TestStringInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput("../../../Common/northwind.xml");
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestStreamInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			using (FileStream fs = File.OpenRead("../../../Common/northwind.xml"))
            {
                XmlInput input = new XmlInput(fs);
                MemoryStream ms = new MemoryStream();
                xslt.Transform(input, Arguments, new XmlOutput(ms));
                CompareResults(standardResult, ms.ToArray());
            }            
        }

        [TestMethod]
        public void TestTextReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput(new StreamReader("../../../Common/northwind.xml", Encoding.GetEncoding("windows-1252")));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestXmlReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput(XmlReader.Create("../../../Common/northwind.xml"));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }


        [TestMethod]
        public void TestIXPathNavigableInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput(new XPathDocument("../../../Common/northwind.xml", XmlSpace.Preserve));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestStringInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput("../../../Common/northwind.xml");
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestStreamInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			using (FileStream fs = File.OpenRead("../../../Common/northwind.xml"))
            {
                XmlInput input = new XmlInput(fs);
                MemoryStream ms = new MemoryStream();
                XmlReader r = xslt.Transform(input, Arguments);
                XmlWriter w = XmlWriter.Create(ms);
                w.WriteNode(r, false);
                w.Close();
                CompareResults(standardResult, ms.ToArray());
            }
        }

        [TestMethod]
        public void TestTextReaderInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput(new StreamReader("../../../Common/northwind.xml", Encoding.GetEncoding("windows-1252")));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestXmlReaderInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
			XmlInput input = new XmlInput(XmlReader.Create("../../../Common/northwind.xml"));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }


        [TestMethod]
        public void TestIXPathNavigableInput2()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput(new XPathDocument("../../../Common/northwind.xml", XmlSpace.Preserve));
            MemoryStream ms = new MemoryStream();
            XmlReader r = xslt.Transform(input, Arguments);
            XmlWriter w = XmlWriter.Create(ms);
            w.WriteNode(r, false);
            w.Close();
            CompareResults(standardResult, ms.ToArray());
        }

        [TestMethod]
        public void TestStringOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../../Common/northwind.xml");
            xslt.Transform(input, Arguments, new XmlOutput("../../../Common/MvpXslTransformTests/out.xml"));
            using (FileStream fs = File.OpenRead("../../../Common/MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }            
        }

        [TestMethod]
        public void TestStreamOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../../Common/northwind.xml");
            using (FileStream outStrm = File.OpenWrite("../../../Common/MvpXslTransformTests/out.xml")) {
                xslt.Transform(input, Arguments, new XmlOutput(outStrm));
            }
            using (FileStream fs = File.OpenRead("../../../Common/MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }

        [TestMethod]
        public void TestTextWriterOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../../Common/northwind.xml");
            TextWriter w = new StreamWriter("../../../Common/MvpXslTransformTests/out.xml", false, Encoding.UTF8);
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();
            using (FileStream fs = File.OpenRead("../../../Common/MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }

        [TestMethod]
        public void TestXmlWriterOutput()
        {
            MvpXslTransform xslt = GetMvpXslTransform();
            XmlInput input = new XmlInput("../../../Common/northwind.xml");
            XmlWriter w = XmlWriter.Create("../../../Common/MvpXslTransformTests/out.xml");
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();
            using (FileStream fs = File.OpenRead("../../../Common/MvpXslTransformTests/out.xml"))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                CompareResults(standardResult, bytes);
            }
        }

        [TestMethod]        
        public void ResolverTestStringInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput("../../../Common/MvpXslTransformTests/test.xml", new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());            
        }

        [TestMethod]
        public void ResolverTestStreamInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            using (FileStream fs = File.OpenRead("../../../Common/MvpXslTransformTests/test.xml"))
            {
                XmlInput input = new XmlInput(fs, new MyXmlResolver());
                MemoryStream ms = new MemoryStream();
                xslt.Transform(input, Arguments, new XmlOutput(ms));
                CompareResults(resolverTestStandardResult, ms.ToArray());
            }
        }

        [TestMethod]
        public void ResolverTestTextReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(new StreamReader("../../../Common/MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [TestMethod]
        public void ResolverTestXmlReaderInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(XmlReader.Create("../../../Common/MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [TestMethod]
        public void ResolverTestIXPathNavigableInput()
        {
            MvpXslTransform xslt = GetMvpXslTransform2();
            XmlInput input = new XmlInput(new XPathDocument("../../../Common/MvpXslTransformTests/test.xml"), new MyXmlResolver());
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            CompareResults(resolverTestStandardResult, ms.ToArray());
        }

        [TestMethod]
        public void ExsltTest()
        {
            MvpXslTransform xslt = new MvpXslTransform();
            xslt.Load("../../../Common/MvpXslTransformTests/exslt-test.xslt");
            XmlInput input = new XmlInput("../../../Common/MvpXslTransformTests/test.xml");
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            string expected = "<out>3</out>";            
            CompareResults(Encoding.ASCII.GetBytes(expected), ms.ToArray());
        }

        [TestMethod]       
        public void NoExsltTest()
        {
            MvpXslTransform xslt = new MvpXslTransform();
            xslt.Load("../../../Common/MvpXslTransformTests/exslt-test.xslt");
            XmlInput input = new XmlInput("../../../Common/MvpXslTransformTests/test.xml");
            MemoryStream ms = new MemoryStream();
            xslt.SupportedFunctions = Mvp.Xml.Exslt.ExsltFunctionNamespace.None;
            try
            {
                xslt.Transform(input, Arguments, new XmlOutput(ms));
            }
            catch (XsltException) { return; }
            Assert.Fail("Shoudn't be here.");
        }

        [TestMethod]
        public void CharMapTest()
        {
            const string expected = "<out attr=\"a&nbsp;b\"><text>Some&nbsp;text, now ASP.NET <%# Eval(\"foo\") %> and more&nbsp;text.</text><foo attr=\"<data>\">text <%= fff() %> and more&nbsp;text.</foo></out>";

            MvpXslTransform xslt = new MvpXslTransform {SupportCharacterMaps = true};
            xslt.Load("../../../Common/MvpXslTransformTests/char-map.xslt");
            XmlInput input = new XmlInput(new StringReader("<foo attr=\"{data}\">text {%= fff() %} and more&#xA0;text.</foo>"));
            StringWriter sw = new StringWriter();            
            xslt.Transform(input, Arguments, new XmlOutput(sw));

            Assert.AreEqual(expected, sw.ToString());
        }

        [TestMethod]
        public void CharMapTest2()
        {
            const string expected = "<out attr=\"a&nbsp;b\"><text>Some&nbsp;text, now ASP.NET <%# Eval(\"foo\") %> and more&nbsp;text.</text><foo attr=\"<data>\">text <%= fff() %> and more&nbsp;text.</foo></out>";

            MvpXslTransform xslt = new MvpXslTransform {SupportCharacterMaps = true};
            xslt.Load("../../../Common/MvpXslTransformTests/char-map.xslt");
            XmlInput input = new XmlInput(new StringReader("<foo attr=\"{data}\">text {%= fff() %} and more&#xA0;text.</foo>"));
            MemoryStream ms = new MemoryStream();
            xslt.Transform(input, Arguments, new XmlOutput(ms));
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms); 
            
            Assert.AreEqual(expected, sr.ReadToEnd());
        }

        [TestMethod]
        public void CharMapTest3()
        {
            const string expected = "<out attr=\"a&nbsp;b\"><text>Some&nbsp;text, now ASP.NET <%# Eval(\"foo\") %> and more&nbsp;text.</text><foo attr=\"<data>\">text <%= fff() %> and more&nbsp;text.</foo></out>";

            MvpXslTransform xslt = new MvpXslTransform {SupportCharacterMaps = true};
            xslt.Load("../../../Common/MvpXslTransformTests/char-map.xslt");
            XmlInput input = new XmlInput(new StringReader("<foo attr=\"{data}\">text {%= fff() %} and more&#xA0;text.</foo>"));
            StringWriter sw = new StringWriter();
            XmlWriter w = XmlWriter.Create(sw, xslt.OutputSettings);
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();

            Assert.AreEqual(expected, sw.ToString());
        }


        [TestMethod]
        public void CharMapTest4()
        {
            const string expected = "<out attr=\"a&nbsp;b\"><text>Some&nbsp;text, now ASP.NET <%# Eval(\"foo\") %> and more&nbsp;text.</text><foo attr=\"<data>\">text <%= fff() %> and more&nbsp;text.</foo></out>";

            MvpXslTransform xslt = new MvpXslTransform {SupportCharacterMaps = true};
            xslt.Load(XmlReader.Create("../../../Common/MvpXslTransformTests/char-map.xslt"));
            XmlInput input = new XmlInput(new StringReader("<foo attr=\"{data}\">text {%= fff() %} and more&#xA0;text.</foo>"));
            StringWriter sw = new StringWriter();
            xslt.Transform(input, Arguments, new XmlOutput(sw));

            Assert.AreEqual(expected, sw.ToString());
        }
        
        [TestMethod]
        public void CharMapTest6()
        {
            const string expected = "<out attr=\"a&nbsp;b\"><text>Some&nbsp;text, now ASP.NET <%# Eval(\"foo\") %> and more&nbsp;text.</text><foo attr=\"<data>\">text <%= fff() %> and more&nbsp;text.</foo></out>";

            MvpXslTransform xslt = new MvpXslTransform {SupportCharacterMaps = true};
            XPathDocument d = new XPathDocument("../../../Common/MvpXslTransformTests/char-map.xslt");
            xslt.Load(d);
            XmlInput input = new XmlInput(new StringReader("<foo attr=\"{data}\">text {%= fff() %} and more&#xA0;text.</foo>"));
            StringWriter sw = new StringWriter();
            xslt.Transform(input, Arguments, new XmlOutput(sw));

            Assert.AreEqual(expected, sw.ToString());
        }

        [TestMethod]
        public void XhtmlTest()
        {
            MvpXslTransform xslt = new MvpXslTransform();
            xslt.Load("../../../Common/MvpXslTransformTests/xhtml.xslt");
            xslt.EnforceXHTMLOutput = true;
            XmlInput input = new XmlInput(new StringReader("<foo/>"));
            StringWriter sw = new StringWriter();
            XmlWriter w = XmlWriter.Create(sw, xslt.OutputSettings);
            xslt.Transform(input, Arguments, new XmlOutput(w));
            w.Close();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(sw.ToString(), "<?xml version=\"1.0\" encoding=\"utf-16\"?><!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>Page title</title></head><body><p>Para</p><p></p><br /><p><img src=\"ddd\" /></p></body></html>");
        }
    }

    public class MyXmlResolver : XmlUrlResolver
    {
        public MyXmlResolver() {}

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri.Scheme == "my")
            {
                string xml = "<resolver>data</resolver>";
                return XmlReader.Create(new StringReader(xml));
            }
            else
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }
        }
    }
}
