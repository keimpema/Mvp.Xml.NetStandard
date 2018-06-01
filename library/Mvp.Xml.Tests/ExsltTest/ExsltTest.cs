//using System;
//using Mvp.Xml.Exslt;
//using System.Xml;
//using System.Xml.XPath;
//using System.IO;

//namespace ExsltTest
//{
//	/// <summary>
//	/// Primitive command-line EXSLT-aware utility.
//	/// </summary>
//    public class ExsltTest
//    {
//		public static void Main(string[] args) {
//            //GDNDynamicTests t = new GDNDynamicTests();
//            //t.EvaluateTest();
//            if (args.Length != 3)
//            {
//                PrintUsage();
//                return;
//            }
//            try
//            {
//                XPathDocument doc = new XPathDocument(args[0]);
//                ExsltTransform xslt = new ExsltTransform();
//                xslt.Load(args[1]);
//                xslt.MultiOutput = false;
//                using (FileStream fs = File.Create(args[2]))
//                {
//                    xslt.Transform(doc, null, fs);
//                }
//            }
//            catch (Exception e)
//            {
//                Console.Error.WriteLine("An exception occured: ");
//                Console.Error.WriteLine(e);
//            }
//		}		
		
//		private static void PrintUsage() {
//		    Console.WriteLine("ExsltTest usage:");
//		    Console.WriteLine("ExsltTest.exe source stylesheet result");
//		}
//	}
//}
