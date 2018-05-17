#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Xml.Common;

namespace Mvp.Xml.Tests.Common
{
	[TestClass]
	public class XmlNormalizingReaderFixture : TestFixtureBase
	{
		[TestMethod]
		public void HidesDuplicateNamespace()
		{
			string source = "<item xmlns:sx='sse'><data xmlns:sx='sse' id='3'/></item>";
			XmlNormalizingReader reader = new XmlNormalizingReader(GetReader(source));

			reader.MoveToContent();
			Assert.AreEqual(1, reader.AttributeCount);
			Assert.IsTrue(reader.MoveToFirstAttribute());

			reader.MoveToElement();
			reader.Read();

			Assert.AreEqual(1, reader.AttributeCount);
			Assert.IsTrue(reader.MoveToFirstAttribute());
			Assert.AreEqual("id", reader.LocalName);
			Assert.AreEqual("3", reader.Value);
			Assert.IsFalse(reader.MoveToNextAttribute());
		}

		[TestMethod]
		public void ReaderDoesNotReportDuplicateNamespaces()
		{
			string source = @"
		<item xmlns:sx='http://www.microsoft.com/schemas/rss/sse' xmlns:sa3='http://www.microsoft.com/schemas/sa3/request' xmlns:geo='geo-tagging'>
				<sx:sync id='101' version='2' deleted='false' noconflicts='false' xmlns:sx='http://www.microsoft.com/schemas/rss/sse'/>
				<title>12345fgcomputers, projectors, ptz cameras, and PC speakerphones for video wall</title>
				<sa3:Data xmlns:sa3='http://www.microsoft.com/schemas/sa3/request'>
				  <sa3:ID>Robert Kirkpatrick/Groove_Sun, 23 Jul 2006 04:07:46 GMT_65576216301.309654</sa3:ID>
				  <unknown-element xmlns='kzu-unknown'/>
				</sa3:Data>
				<geo:location xmlns:geo='geo-tagging'>
				  <geo:latitude>120</geo:latitude>
				</geo:location>
				<sa3:Info xmlns:sa3='http://www.microsoft.com/schemas/sa3/request'>kzu</sa3:Info>
		</item>
				";

			string expected = NormalizeFormat(@"
		<item xmlns:sx='http://www.microsoft.com/schemas/rss/sse' xmlns:sa3='http://www.microsoft.com/schemas/sa3/request' xmlns:geo='geo-tagging'>
				<sx:sync id='101' version='2' deleted='false' noconflicts='false'/>
				<title>12345fgcomputers, projectors, ptz cameras, and PC speakerphones for video wall</title>
				<sa3:Data>
				  <sa3:ID>Robert Kirkpatrick/Groove_Sun, 23 Jul 2006 04:07:46 GMT_65576216301.309654</sa3:ID>
				  <unknown-element xmlns='kzu-unknown'/>
				</sa3:Data>
				<geo:location>
				  <geo:latitude>120</geo:latitude>
				</geo:location>
				<sa3:Info>kzu</sa3:Info>
		</item>
				");

			XmlNormalizingReader reader = new XmlNormalizingReader(GetReader(source));
			string actual = ReadToEnd(reader);

			Assert.AreEqual(expected, actual);
		}
	}
}
