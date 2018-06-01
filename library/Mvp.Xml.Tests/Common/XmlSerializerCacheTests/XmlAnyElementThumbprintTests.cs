#region Using directives

using System;
using System.Collections;
using System.Text;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using System.Xml.Serialization;
using Mvp.Xml.Common.Serialization;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestClass]
	public class XmlAnyElementThumbprintTests
	{
		public XmlAnyElementThumbprintTests()
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
		public void AnyElementAttributesSameMember()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute();
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute();

			atts1.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void AnyElementAttributesSameNamespace()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute("myname", "myns");

			atts1.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNamespacesAnyElementAttributes()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute("myname", "myotherns");

			atts1.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNamesAnyElementAttributes()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute("myothername", "myns");

			atts1.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any2);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void AnyElementAttributeDifferentMember()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");

			atts1.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any1);

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "AnotherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void TwoSameAnyElement()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute("myothername", "myns");

			atts1.XmlAnyElements.Add(any1);
			atts1.XmlAnyElements.Add(any2);

			atts2.XmlAnyElements.Add(any1);
			atts2.XmlAnyElements.Add(any2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void TwoDifferentAnyElement()
		{
			XmlAnyElementAttribute any1 = new XmlAnyElementAttribute("myname", "myns");
			XmlAnyElementAttribute any2 = new XmlAnyElementAttribute("myothername", "myns");
			XmlAnyElementAttribute any3 = new XmlAnyElementAttribute("mythirdname", "my3ns");

			atts1.XmlAnyElements.Add(any1);
			atts1.XmlAnyElements.Add(any2);

			atts2.XmlAnyElements.Add(any3);
			atts2.XmlAnyElements.Add(any2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
