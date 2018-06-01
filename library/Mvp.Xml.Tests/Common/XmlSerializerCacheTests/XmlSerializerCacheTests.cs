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
	public class XmlSerializerCacheTests
	{
		private bool NewInstaceCreated;
		private bool CacheHit;
		XmlSerializerCache cache;

		public XmlSerializerCacheTests()
		{

		}

		[TestInitialize]
		public void SetUp()
		{
			cache = new XmlSerializerCache();
			ClearFlags();
			ConnectListeners();
		}

		[TestCleanup]
		public void TearDown()
		{
			DisonnectListeners();
			cache.Dispose();
			cache = null;
		}

		public void ClearFlags()
		{
			NewInstaceCreated = false;
			CacheHit = false;
		}

		private void ConnectListeners()
		{
			cache.NewSerializer += new SerializerCacheDelegate(cache_NewSerializer);
			cache.CacheHit += new SerializerCacheDelegate(cache_CacheHit);
		}

		private void DisonnectListeners()
		{
			cache.NewSerializer -= new SerializerCacheDelegate(cache_NewSerializer);
			cache.CacheHit -= new SerializerCacheDelegate(cache_CacheHit);
		}
		[TestMethod]
		public void AllParams()
		{
			Type[] types1 = new Type[] { typeof(SerializeMe), typeof(SerializeMeToo) };
			Type[] types2 = new Type[] { typeof(SerializeMe), typeof(SerializeMeToo) };
			XmlAttributeOverrides over1 = new XmlAttributeOverrides();
			XmlAttributeOverrides over2 = new XmlAttributeOverrides();
			XmlAttributes atts1 = new XmlAttributes();
			XmlAttributes atts2 = new XmlAttributes();

			atts1.XmlType = new XmlTypeAttribute("mytype");
			atts2.XmlType = new XmlTypeAttribute("mytype");

			over1.Add(typeof(SerializeMe), atts1);
			over2.Add(typeof(SerializeMe), atts2);

			XmlRootAttribute root1 = new XmlRootAttribute("someelement");
			XmlRootAttribute root2 = new XmlRootAttribute("someelement");

			string namespace1 = "mynamespace";
			string namespace2 = "mynamespace";

			System.Xml.Serialization.XmlSerializer ser1 = cache.GetSerializer(typeof(SerializeMe)
				, over1
				, types1
				, root1
				, namespace1);


			Assert.AreEqual(false, CacheHit);
			Assert.AreEqual(true, NewInstaceCreated);
			ClearFlags();

			System.Xml.Serialization.XmlSerializer ser2 = cache.GetSerializer(typeof(SerializeMe)
				, over2
				, types2
				, root2
				, namespace2);


			Assert.AreEqual(true, CacheHit);
			Assert.AreEqual(false, NewInstaceCreated);

			Assert.AreSame(ser1, ser2);
		}

		[TestMethod]
		public void TypesParam()
		{
			Type[] types = new Type[] { typeof(SerializeMe), typeof(SerializeMeToo) };

			System.Xml.Serialization.XmlSerializer ser1 = cache.GetSerializer(typeof(SerializeMe)
				, types);

			Assert.AreEqual(false, CacheHit);
			Assert.AreEqual(true, NewInstaceCreated);
			ClearFlags();

			System.Xml.Serialization.XmlSerializer ser2 = cache.GetSerializer(typeof(SerializeMe)
				, types);

			Assert.AreEqual(true, CacheHit);
			Assert.AreEqual(false, NewInstaceCreated);

			Assert.AreSame(ser1, ser2);
		}

		[TestMethod]
		public void OverridesParam()
		{
			XmlAttributeOverrides over1 = new XmlAttributeOverrides();
			XmlAttributeOverrides over2 = new XmlAttributeOverrides();
			XmlAttributes atts1 = new XmlAttributes();
			XmlAttributes atts2 = new XmlAttributes();

			atts1.XmlType = new XmlTypeAttribute("mytype");
			atts2.XmlType = new XmlTypeAttribute("mytype");

			over1.Add(typeof(SerializeMe), atts1);
			over2.Add(typeof(SerializeMe), atts2);

			System.Xml.Serialization.XmlSerializer ser1 = cache.GetSerializer(typeof(SerializeMe)
					, over1);
			Assert.AreEqual(false, CacheHit);
			Assert.AreEqual(true, NewInstaceCreated);
			ClearFlags();

			System.Xml.Serialization.XmlSerializer ser2 = cache.GetSerializer(typeof(SerializeMe)
				, over2);
			Assert.AreEqual(true, CacheHit);
			Assert.AreEqual(false, NewInstaceCreated);

			Assert.AreSame(ser1, ser2);
		}

		[TestMethod]
		public void RootParam()
		{
			XmlRootAttribute root1 = new XmlRootAttribute("someelement");
			XmlRootAttribute root2 = new XmlRootAttribute("someelement");
			System.Xml.Serialization.XmlSerializer ser1 = cache.GetSerializer(typeof(SerializeMe)
					, root1);
			Assert.AreEqual(false, CacheHit);
			Assert.AreEqual(true, NewInstaceCreated);
			ClearFlags();

			System.Xml.Serialization.XmlSerializer ser2 = cache.GetSerializer(typeof(SerializeMe)
				, root2);
			Assert.AreEqual(true, CacheHit);
			Assert.AreEqual(false, NewInstaceCreated);

			Assert.AreSame(ser1, ser2);
		}

		[TestMethod]
		public void NamespaceParam()
		{
			string namespace1 = "mynamespace";
			string namespace2 = "mynamespace";

			System.Xml.Serialization.XmlSerializer ser1 = ser1 = cache.GetSerializer(typeof(SerializeMe)
					, namespace1);
			Assert.AreEqual(false, CacheHit);
			Assert.AreEqual(true, NewInstaceCreated);
			ClearFlags();

			System.Xml.Serialization.XmlSerializer ser2 = cache.GetSerializer(typeof(SerializeMe)
				, namespace2);
			Assert.AreEqual(true, CacheHit);
			Assert.AreEqual(false, NewInstaceCreated);

			Assert.AreSame(ser1, ser2);
		}


		void cache_NewSerializer(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, String defaultNamespace)
		{
			NewInstaceCreated = true;
		}

		void cache_CacheHit(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, String defaultNamespace)
		{
			CacheHit = true;
		}
	}
}
