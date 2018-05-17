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
	public class XmlTextThumbprintTests
	{
		public XmlTextThumbprintTests()
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
		public void SameType()
		{
			XmlTextAttribute text1 = new XmlTextAttribute(typeof(SerializeMe));
			XmlTextAttribute text2 = new XmlTextAttribute(typeof(SerializeMe));

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1,ov2);
		}

		[TestMethod]
		public void DifferentTypes()
		{
			XmlTextAttribute text1 = new XmlTextAttribute(typeof(SerializeMe));
			XmlTextAttribute text2 = new XmlTextAttribute(typeof(SerializeMeToo));

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}

		[TestMethod]
		public void SameDataType()
		{
			XmlTextAttribute text1 = new XmlTextAttribute();
			text1.DataType = "xmltype1";
			XmlTextAttribute text2 = new XmlTextAttribute();
			text2.DataType = "xmltype1";

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentDataTypes()
		{
			XmlTextAttribute text1 = new XmlTextAttribute();
			text1.DataType = "xmltype1";
			XmlTextAttribute text2 = new XmlTextAttribute();
			text2.DataType = "xmltype2";

			atts1.XmlText = text1;
			atts2.XmlText = text2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}
	}
}
