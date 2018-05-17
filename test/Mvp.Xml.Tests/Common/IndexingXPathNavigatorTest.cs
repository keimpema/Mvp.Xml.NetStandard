using System;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

using Mvp.Xml.Common.XPath;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Mvp.Xml.Tests
{
	/// <summary>
	/// Test class for IndexingXPathNavigator
	/// </summary>
	[TestClass]
	public class IndexingXPathNavigatorTest
	{
		[TestMethod]
		public void RunTests()
		{
			Main2(new string[0]);
		}

		[STAThread]
		static void Main2(string[] args)
		{           
            Stopwatch stopWatch = new Stopwatch();
			int repeat = 1000;
			stopWatch.Start();
			XPathDocument doc = new XPathDocument(Globals.GetResource(Globals.NorthwindResource));
			//XmlDocument doc = new XmlDocument();
			//doc.Load("test/northwind.xml");
            stopWatch.Stop();            
			Console.WriteLine("Loading XML document: {0, 6:f2} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
			XPathNavigator nav = doc.CreateNavigator();      
			XPathExpression expr = nav.Compile("/ROOT/CustomerIDs/OrderIDs/Item[OrderID=' 10330']/ShipAddress");
      
			Console.WriteLine("Regular selection, warming...");
			SelectNodes(nav, repeat, stopWatch, expr);
			Console.WriteLine("Regular selection, testing...");
			SelectNodes(nav, repeat, stopWatch, expr);
   
      
			stopWatch.Start();
			IndexingXPathNavigator inav = new IndexingXPathNavigator(
				doc.CreateNavigator());
            stopWatch.Stop();
			Console.WriteLine("Building IndexingXPathNavigator: {0, 6:f2} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
			stopWatch.Start();
			inav.AddKey("orderKey", "OrderIDs/Item", "OrderID");
            stopWatch.Stop();
			Console.WriteLine("Adding keys: {0, 6:f2} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
			XPathExpression expr2 = inav.Compile("key('orderKey', ' 10330')/ShipAddress");
			stopWatch.Start();
			inav.BuildIndexes();
            stopWatch.Stop();
			Console.WriteLine("Indexing: {0, 6:f2} ms", stopWatch.ElapsedMilliseconds);
            stopWatch.Reset();
      
			Console.WriteLine("Indexed selection, warming...");
			SelectIndexedNodes(inav, repeat, stopWatch, expr2);
			Console.WriteLine("Indexed selection, testing...");
			SelectIndexedNodes(inav, repeat, stopWatch, expr2);      
		}

		private static void SelectNodes(XPathNavigator nav, int repeat, Stopwatch stopWatch, XPathExpression expr) 
		{
			int counter = 0;
            stopWatch.Start();
			for (int i=0; i<repeat; i++) 
			{
				XPathNodeIterator ni =  nav.Select(expr);
				while (ni.MoveNext())
					counter++;
			}
            stopWatch.Stop();
			Console.WriteLine("Regular selection: {0} times, total time {1, 6:f2} ms, {2} nodes selected", repeat,
                stopWatch.ElapsedMilliseconds, counter);
            stopWatch.Reset();
		}

        private static void SelectIndexedNodes(XPathNavigator nav, int repeat, Stopwatch stopWatch, XPathExpression expr) 
		{
			int counter = 0;
			stopWatch.Start();
			for (int i=0; i<repeat; i++) 
			{
				XPathNodeIterator ni =  nav.Select(expr);
				while (ni.MoveNext())
					counter++;
			}
            stopWatch.Stop();
			Console.WriteLine("Indexed selection: {0} times, total time {1, 6:f2} ms, {2} nodes selected", repeat, 
				stopWatch.ElapsedMilliseconds, counter);
            stopWatch.Reset();
		}
	}
}