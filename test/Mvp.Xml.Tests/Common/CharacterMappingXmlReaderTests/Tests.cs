#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Data;

using Mvp.Xml.Common.Xsl;
using Mvp.Xml.Tests;


namespace Mvp.Xml.Tests.CharacterMappingXmlReaderTests
{
    [TestClass]
    public class Tests
    {
        public static CharacterMappingXmlReader GetReader()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        public static CharacterMappingXmlReader GetReader2()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style2.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        public static CharacterMappingXmlReader GetReader3()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style3.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        public static CharacterMappingXmlReader GetReader4()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style4.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        public static CharacterMappingXmlReader GetReader5()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style5.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        public static CharacterMappingXmlReader GetReader6()
        {
            XmlReader baseReader = XmlReader.Create("../../../Common/CharacterMappingXmlReaderTests/style6.xslt");
            return new CharacterMappingXmlReader(baseReader);
        }

        [TestMethod]
        public void TestReaderShoulReadCharMap()
        {
            CharacterMappingXmlReader r = GetReader();
            while (r.Read()) ;
            Dictionary<char, string> map = r.CompileCharacterMapping();
            Assert.IsNotNull(map);
            Assert.IsTrue(map.ContainsKey('\u00A0'));
            Assert.IsTrue(map['\u00A0']== "&nbsp;");
        }        

        [TestMethod]
        public void TestReaderShoulReadAllMaps()
        {
            CharacterMappingXmlReader r = GetReader2();
            while (r.Read()) ;
            Dictionary<char, string> map = r.CompileCharacterMapping();
            Assert.IsNotNull(map);                        
            Assert.IsTrue(map.ContainsKey('\u00A0'));
            Assert.IsTrue(map['\u00A0'] == "&nbsp;");                        
            Assert.IsTrue(map.ContainsKey('\u00A1'));
            Assert.IsTrue(map['\u00A1'] == "161");
            Assert.IsTrue(map.ContainsKey('\u00A2'));
            Assert.IsTrue(map['\u00A2'] == "162");
        }

        [TestMethod]
        public void TestReaderShouldCompileSingleMap()
        {
            CharacterMappingXmlReader r = GetReader3();
            while (r.Read()) ;
            Dictionary<char, string> map = r.CompileCharacterMapping();
            Assert.IsNotNull(map);
            Assert.IsTrue(map.ContainsKey('\u00A0'));
            Assert.IsTrue(map['\u00A0'] == "&nbsp;");
            Assert.IsTrue(map.ContainsKey('\u00A1'));
            Assert.IsTrue(map['\u00A1'] == "161");
            Assert.IsTrue(map.ContainsKey('\u00A2'));
            Assert.IsTrue(map['\u00A2'] == "162");            
        }

        [TestMethod]
        public void TestDuplicate()
        {
            CharacterMappingXmlReader r = GetReader5();
          try 
	        {	        
		    while (r.Read());
              Assert.Fail("Must be exception here.");
	        }
	        catch (Exception e)
	        {
                Console.WriteLine(e);
	        }
        }

        [TestMethod]
        public void TestLoop()
        {
            CharacterMappingXmlReader r = GetReader4();
            while (r.Read());
            try
            {
                Dictionary<char, string> map = r.CompileCharacterMapping();
                Assert.Fail("Should be exception");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [TestMethod]
        public void TestOverride()
        {
            CharacterMappingXmlReader r = GetReader6();
            while (r.Read()) ;
            Dictionary<char, string> map = r.CompileCharacterMapping();
            Assert.IsNotNull(map);
            Assert.IsTrue(map.ContainsKey('\u00A0'));            
            Assert.IsTrue(map['\u00A0'] == "&nbsp2;");
            Assert.IsTrue(map.ContainsKey('\u00A1'));
            Assert.IsTrue(map['\u00A1'] == "161");            
        }
    }
}
