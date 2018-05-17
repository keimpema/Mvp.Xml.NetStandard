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
	public class XmlDefaultValuePrintTest
	{
		public XmlDefaultValuePrintTest()
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
		public void DifferentValue()
		{
			string val1 = "bla";
			int val2 = 1;

			atts1.XmlDefaultValue = val1;
			atts2.XmlDefaultValue = val2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameValue()
		{
			string val1 = "bla";
			string val2 = "bla";

			atts1.XmlDefaultValue = val1;
			atts2.XmlDefaultValue = val2;

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

	}
}
