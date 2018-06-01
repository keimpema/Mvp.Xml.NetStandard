#region using

using System;
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

#endregion using 

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// </summary>
	[TestClass]
	public class XmlNodeFactoryTests
	{
		static XmlSerializer ser = new XmlSerializer(typeof(XmlNode));

		[TestMethod]
		public void NodeFromReader()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><element>1</element><element></element><element>2</element></root>";
			XmlTextReader tr = new XmlTextReader(new StringReader(xml));

			XmlNode node = XmlNodeFactory.Create(tr);
			MemoryStream mem = new MemoryStream();
			XmlTextWriter tw = new XmlTextWriter(mem, System.Text.Encoding.UTF8);
			tw.Formatting = Formatting.None;

			ser.Serialize(tw, node);
			mem.Position = 0;
			string res = new StreamReader(mem).ReadToEnd();

			Assert.AreEqual(xml, res);
		}


		[TestMethod]
		public void NodeFromNavigator()
		{
			string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><element>1</element><element /><element>2</element></root>";
			XPathDocument doc = new XPathDocument(new StringReader(xml));

			XPathNavigator nav = doc.CreateNavigator();
			XmlNode node = XmlNodeFactory.Create(nav);
			MemoryStream mem = new MemoryStream();
			XmlTextWriter tw = new XmlTextWriter(mem, System.Text.Encoding.UTF8);
			tw.Formatting = Formatting.None;

			ser.Serialize(tw, node);
			mem.Position = 0;
			string res = new StreamReader(mem).ReadToEnd();

			Assert.AreEqual(xml, res);

			nav.MoveToRoot();
			nav.MoveToFirstChild();
			nav.MoveToFirstChild();

			node = XmlNodeFactory.Create(nav);
			mem = new MemoryStream();
			tw = new XmlTextWriter(mem, System.Text.Encoding.UTF8);
			tw.Formatting = Formatting.None;

			ser.Serialize(tw, node);
			mem.Position = 0;
			res = new StreamReader(mem).ReadToEnd();

			Assert.AreEqual("<?xml version=\"1.0\" encoding=\"utf-8\"?><element>1</element>", res);
		}

		[TestMethod]
		public void NodeFromObject()
		{
			Customer cust = new Customer();
			cust.FirstName = "Daniel";
			cust.LastName = "Cazzulino";

			XmlNode node = XmlNodeFactory.Create(cust);
			MemoryStream mem = new MemoryStream();
			XmlTextWriter tw = new XmlTextWriter(mem, System.Text.Encoding.UTF8);
			tw.Formatting = Formatting.None;

			ser.Serialize(tw, node);
			mem.Position = 0;

			XmlSerializer customerSerializer = new XmlSerializer(typeof(Customer));
			Customer result = (Customer) customerSerializer.Deserialize(mem);

			Assert.AreEqual(cust.FirstName, result.FirstName);
			Assert.AreEqual(cust.LastName, result.LastName);
			Assert.AreEqual(cust.BirthDate, result.BirthDate);
		}

		public class Customer
		{
			public string FirstName;
			public string LastName;
			public DateTime BirthDate;
		}
	}
}
