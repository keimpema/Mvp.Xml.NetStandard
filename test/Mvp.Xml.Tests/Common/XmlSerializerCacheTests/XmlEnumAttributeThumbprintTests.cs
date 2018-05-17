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
	public class XmlEnumAttributeThumbprintTests
	{
		public XmlEnumAttributeThumbprintTests()
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
		public void SameName()
		{
			XmlEnumAttribute enum1 = new XmlEnumAttribute("enum1");
			XmlEnumAttribute enum2 = new XmlEnumAttribute("enum1");

			atts1.XmlEnum = enum1;
			atts2.XmlEnum = enum2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentName()
		{
			XmlEnumAttribute enum1 = new XmlEnumAttribute("enum1");
			XmlEnumAttribute enum2 = new XmlEnumAttribute("enum2");

			atts1.XmlEnum = enum1;
			atts2.XmlEnum = enum2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
