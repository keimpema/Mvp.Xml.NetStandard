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

using Mvp.Xml.Common.Serialization;
using System.Xml.Serialization;
#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestClass]
	public class XmlArrayThumbprintTests
	{
		public XmlArrayThumbprintTests()
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
		public void XmlArraySameName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentTypes()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentNames()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myothername");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArraySameNamespace()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Namespace = "mynamespace";

			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Namespace = "mynamespace";

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentNamespace()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Namespace = "mynamespace";
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Namespace = "myothernamespace";

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArraySameNullable()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.IsNullable = true;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.IsNullable = true;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentNullable()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.IsNullable = true;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.IsNullable = false;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArraySameForm()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Qualified;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentForm()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Unqualified;

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArraySameMemberName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void XmlArrayDifferentMemberName()
		{
			XmlArrayAttribute array1 = new XmlArrayAttribute("myname");
			XmlArrayAttribute array2 = new XmlArrayAttribute("myname");

			atts1.XmlArray = array1;
			atts2.XmlArray = array2;

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheOtherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}
	}
}
