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

using Mvp.Xml.Serialization;
using System.Xml.Serialization;

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestClass]
	public class XmlArrayItemThumbprintTests
	{
		public XmlArrayItemThumbprintTests()
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
		public void SameItemType()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentItemTypes()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname", typeof(SerializeMeToo));

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameDataType()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array1.DataType = "myfirstxmltype";
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array2.DataType = "myfirstxmltype";

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentDataTypes()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute();
			array1.DataType = "typeone";
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute();
			array2.DataType = "typetwo";

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameNestingLevel()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array1.NestingLevel = 1;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array2.NestingLevel = 1;

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNestingLevels()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array1.NestingLevel = 1;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname", typeof(SerializeMe));
			array2.NestingLevel = 2;

			atts1.XmlArrayItems.Add(array1);
			atts2.XmlArrayItems.Add(array2);

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameElementName()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentTypes()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMeToo), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentElementNames()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myothername");

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameNamespace()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.Namespace = "mynamespace";

			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.Namespace = "mynamespace";

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNamespace()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.Namespace = "mynamespace";
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.Namespace = "myothernamespace";

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameNullable()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.IsNullable = true;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.IsNullable = true;

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentNullable()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.IsNullable = true;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.IsNullable = false;

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameForm()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Qualified;

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentForm()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			array1.Form = System.Xml.Schema.XmlSchemaForm.Qualified;
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");
			array2.Form = System.Xml.Schema.XmlSchemaForm.Unqualified;

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), atts1);
			ov2.Add(typeof(SerializeMe), atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void SameMemberName()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheMember", atts2);

			ThumbprintHelpers.SameThumbprint(ov1, ov2);
		}

		[TestMethod]
		public void DifferentMemberName()
		{
			XmlArrayItemAttribute array1 = new XmlArrayItemAttribute("myname");
			XmlArrayItemAttribute array2 = new XmlArrayItemAttribute("myname");

			atts1.XmlArrayItems.Add( array1 );
			atts2.XmlArrayItems.Add( array2 );

			ov1.Add(typeof(SerializeMe), "TheMember", atts1);
			ov2.Add(typeof(SerializeMe), "TheOtherMember", atts2);

			ThumbprintHelpers.DifferentThumbprint(ov1, ov2);
		}


	}
}
