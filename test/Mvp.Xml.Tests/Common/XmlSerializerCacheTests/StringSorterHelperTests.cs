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

#endregion

namespace Mvp.Xml.Serialization.Tests
{
	[TestClass]
	public class StringSorterHelperTests
	{
		public StringSorterHelperTests()
		{

		}
		const string s1 = "Alex";
		const string s2 = "Bert";
		const string s3 = "Christoph";

		[TestMethod]
		public void SortStrings()
		{
			StringSorter sorter = new StringSorter();
			sorter.AddString(s3);
			sorter.AddString(s2);
			sorter.AddString(s1);

			AssertStringOrder( sorter.GetOrderedArray() );
		}

		[TestMethod]
		public void SortMoreStrings()
		{
			StringSorter sorter = new StringSorter();
			sorter.AddString(s2);
			sorter.AddString(s1);
			sorter.AddString(s3);

			AssertStringOrder(sorter.GetOrderedArray());
		}

		[TestMethod]
		public void SortStrings2()
		{
			StringSorter sorter = new StringSorter();
			sorter.AddString(s3);
			sorter.AddString(s1);
			sorter.AddString(s2);

			AssertStringOrder(sorter.GetOrderedArray());
		}

		[TestMethod]
		public void SortStrings3()
		{
			StringSorter sorter = new StringSorter();
			sorter.AddString(s1);
			sorter.AddString(s2);
			sorter.AddString(s3);

			AssertStringOrder(sorter.GetOrderedArray());
		}
		private void AssertStringOrder( string[] sortedArray )
		{
			Assert.AreEqual(s1, sortedArray[0]);
			Assert.AreEqual(s2, sortedArray[1]);
			Assert.AreEqual(s3, sortedArray[2]);

		}
	}
}
