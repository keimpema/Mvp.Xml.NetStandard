#region Using directives

using System;
using System.Collections;
using System.Text;
using Mvp.Xml.Common.Serialization;
using System.Xml.Serialization;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestClass]
	public class XmlRootThumbprintTests
	{
		public XmlRootThumbprintTests()
		{

		}

		XmlAttributeOverrides ov1;
		XmlAttributeOverrides ov2;

		XmlAttributes atts1;
		XmlAttributes atts2;

		[TestInitialize]
		public void SetUp()
		{
			ov1 = new XmlAttributeOverrides();
			ov2 = new XmlAttributeOverrides();

			atts1 = new XmlAttributes();
			atts2 = new XmlAttributes();
		}

		[TestMethod]
		public void SameDataType()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.DataType = "myfirstxmltype";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.DataType = "myfirstxmltype";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentDataType()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.DataType = "myfirstxmltype";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.DataType = "mysecondxmltype";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameElementName()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			XmlRootAttribute root2 = new XmlRootAttribute("myname");

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentElementName()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			XmlRootAttribute root2 = new XmlRootAttribute("myothername");

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameNullable()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.IsNullable = true;
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.IsNullable = true;

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNullable()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.IsNullable = true;
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.IsNullable = false;

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameNamespace()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.Namespace = "mynamespace";

			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.Namespace = "mynamespace";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNamespace()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("myname");
			root1.Namespace = "mynamespace";
			XmlRootAttribute root2 = new XmlRootAttribute("myname");
			root2.Namespace = "myothernamespace";

			atts1.XmlRoot = root1;
			atts2.XmlRoot = root2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
