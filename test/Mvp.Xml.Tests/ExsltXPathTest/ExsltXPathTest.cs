//using System;
//using Mvp.Xml.Exslt;
//using System.Xml;
//using System.Xml.XPath;

//namespace ExsltXPathTest
//{
//	/// <summary>
//	/// Test of XPath-only EXSLT.
//	/// </summary>
//	public class ExsltXPathTest {
		
//		public static void Main(string[] args) {
//		    if (args.Length != 2) {
//		        PrintUsage();
//		        return;
//		    }		    		    
//		    XPathDocument doc = new XPathDocument(args[0]);
//		    XPathNavigator nav = doc.CreateNavigator();
//		    XPathExpression expr = nav.Compile(args[1]);
//		    ExsltContext ctxt = new ExsltContext(nav.NameTable);
//		    expr.SetContext(ctxt);
//		    object o = nav.Evaluate(expr);
//		    if (o is XPathNodeIterator) {
//		        XPathNodeIterator ni = (XPathNodeIterator)o;
//		        while (ni.MoveNext()) {
//		            Console.WriteLine("Node - type:{0}, name:{1}, value:{2}", ni.Current.NodeType, 
//		                ni.Current.Name, ni.Current.Value);
//		        }
//		    } else {
//		        Console.WriteLine(o);
//		    }		        
//		}	
		
//		private static void PrintUsage() {
//		    Console.WriteLine("ExsltXPathTest usage:");
//		    Console.WriteLine("ExsltXPathTest.exe source XPath-expression");
//		}	
//	}
//}
