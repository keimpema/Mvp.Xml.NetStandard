using System;
using System.Xml;
using System.Xml.XPath;

using Mvp.Xml.Common.XPath;

namespace Mvp.Xml.Tests
{
	public class XPathCacheUsability
	{
		/// <summary>
		/// This code is just to show how it's used. 
		/// If run it will throw exceptions.
		/// </summary>
		public void Test()
		{
			XPathNavigator document = new XPathDocument(String.Empty).CreateNavigator();
			XPathNodeIterator it = XPathCache.Select("/customer/order/item", document);

			it = XPathCache.Select("/customer/order[id=$id]/item", document, 
				new XPathVariable("id", "23"));

			string[] ids = null;
			foreach (string id in ids)
			{
				it = XPathCache.Select("/customer/order[id=$id]/item", document, 
					new XPathVariable("id", id));
			}
			
			XmlNamespaceManager mgr = new XmlNamespaceManager(document.NameTable);
			mgr.AddNamespace("po", "example-po");

			it = XPathCache.Select("/po:customer[id=$id]", document, mgr,
				new XPathVariable("id", "0736"));

			XmlDocument doc = new XmlDocument();
			XmlNodeList list = XPathCache.SelectNodes("/customer", doc);
		}
	}
}
