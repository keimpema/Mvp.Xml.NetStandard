#region Using directives

using System;
using System.Collections;
using System.Text;
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

using Mvp.Xml.Common;

#endregion

namespace Mvp.Xml.Serialization.Tests
{

	internal class ChoiceIdentifierAttributeProvider : System.Reflection.ICustomAttributeProvider
	{

		string name;
		public ChoiceIdentifierAttributeProvider( string name )
		{
			this.name = name;
		}
#region ICustomAttributeProvider Members

public object[]  GetCustomAttributes(bool inherit)
{
    return new object[] { new XmlChoiceIdentifierAttribute(name) };
}

public object[]  GetCustomAttributes(Type attributeType, bool inherit)
{
	object[] o = null;
	if (attributeType == typeof(XmlChoiceIdentifierAttribute))
	{
		o = new object[1];
		o[0] = new XmlChoiceIdentifierAttribute(name);
	}
	else
	{
		o = new object[0];
	}

	return o;
}

public bool  IsDefined(Type attributeType, bool inherit)
{
	bool retVal = false;
	if( typeof(System.Xml.Serialization.XmlChoiceIdentifierAttribute) == attributeType )
	{
		retVal = true;
	}
 	return retVal;
}

#endregion
}

	[TestClass]
	public class XmlChoiceIndetifierPrintTests
	{
		public XmlChoiceIndetifierPrintTests()
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
		}

		[TestMethod]
		public void SameMemberName()
		{
			atts1 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));
			atts2 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentMemberName()
		{
			atts1 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myname"));
			atts2 = new XmlAttributes(new ChoiceIdentifierAttributeProvider("myothername"));

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);

		}
	}
}
