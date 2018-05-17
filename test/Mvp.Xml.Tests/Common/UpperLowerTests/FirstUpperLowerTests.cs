#region using

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Schema;

using Mvp.Xml.Common;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


#endregion using

namespace Mvp.Xml.Tests.UpperLowerTests
{
	[TestClass]
	public class FirstUpperLowerTests
	{
		[TestMethod]
		public void XmlFirstUpperReader()
		{
			string xml = "<customer id='1' pp:id='aba' xmlns='urn-kzu' xmlns:pp='urn-pepenamespace'><pp:order /><order id='1'>Chocolates</order></customer>";

			XmlFirstUpperReader fr = new XmlFirstUpperReader(new StringReader(xml));
            
			fr.MoveToContent();
			Assert.AreEqual("Customer", fr.LocalName);
			fr.MoveToFirstAttribute();
			Assert.AreEqual("Id", fr.LocalName);
			fr.MoveToNextAttribute();
			Assert.AreEqual("pp:Id", fr.Name);

			// Namespace ordering is not guaranteed.
			fr.MoveToNextAttribute();
			Assert.IsTrue( fr.Name == "xmlns" || fr.Name == "xmlns:pp" );
			fr.MoveToNextAttribute();
			Assert.IsTrue( fr.Name == "xmlns" || fr.Name == "xmlns:pp" );

			fr.MoveToElement();
			fr.Read();
			Assert.AreEqual("pp:Order", fr.Name);
		}

		[TestMethod]
		public void XmlFirstLowerWriter()
		{
			string xml = "<Customer Id=\"1\" pp:Id=\"aba\" xmlns=\"urn-kzu\" xmlns:pp=\"urn-pepenamespace\"><pp:Order /><Order Id=\"1\">chocolates</Order></Customer>";

			XmlTextReader tr = new XmlTextReader(new StringReader(xml));
			
			StringWriter sw = new StringWriter();
			XmlFirstLowerWriter fw = new XmlFirstLowerWriter(sw);
            
			fw.WriteNode(tr, true);
			fw.Flush();

			Assert.AreEqual(xml.ToLower(), sw.ToString());
		}

		[TestMethod]
		public void Deserialization()
		{
            XmlFirstUpperReader fu = new XmlFirstUpperReader("../../../Common/UpperLowerTests/Customer.xml");
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(XmlSchema.Read(XmlReader.Create("../../../Common/UpperLowerTests/Customer.xsd"), null));
			XmlReader vr = XmlReader.Create(fu, settings);			
			XmlSerializer ser = new XmlSerializer(typeof(Customer));
			Customer c = (Customer) ser.Deserialize(vr);

			Assert.AreEqual("0736", c.Id);
			Assert.AreEqual("Daniel Cazzulino", c.Name);
			Assert.AreEqual(25, c.Order.Id);
		}

		[TestMethod]
		public void Serialization()
		{
            XmlFirstUpperReader fu = new XmlFirstUpperReader("../../../Common/UpperLowerTests/Customer.xml");
			XmlSerializer ser = new XmlSerializer(typeof(Customer));
			Customer c = (Customer) ser.Deserialize(fu);

			StringWriter sw = new StringWriter();
			XmlFirstLowerWriter fl = new XmlFirstLowerWriter(sw);

			ser.Serialize(fl, c);

      Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-16\"?><customer xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" id=\"0736\" xmlns=\"mvp-xml-customer\"><name>Daniel Cazzulino</name><order id=\"25\" /></customer>", 
				sw.ToString());
		}
	}
}