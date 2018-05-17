using System;
using System.Xml.XPath;
using System.Xml;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements the EXSLT functions in the http://exslt.org/random namespace.
	/// </summary>
	public class ExsltRandom
	{
		/// <summary>
		///  Implements the following function 
		///     number+ random:random-sequence(number?, number?)
		/// </summary>				
		public XPathNodeIterator randomSequence()
		{
			return randomSequenceImpl(1, (int)DateTime.Now.Ticks);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>	
		public XPathNodeIterator randomSequence_RENAME_ME()
		{
			return randomSequence();
		}

		/// <summary>
		///  Implements the following function 
		///     number+ random:random-sequence(number?, number?)
		/// </summary>	
		public XPathNodeIterator randomSequence(double number)
		{

			return randomSequenceImpl(number, (int)DateTime.Now.Ticks);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>	
		public XPathNodeIterator randomSequence_RENAME_ME(double number)
		{
			return randomSequence(number);
		}

		/// <summary>
		///  Implements the following function 
		///     number+ random:random-sequence(number?, number?)
		/// </summary>
		public XPathNodeIterator randomSequence(double number, double seed)
		{
			return randomSequenceImpl(number, (int)(seed % int.MaxValue));
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>	
		public XPathNodeIterator randomSequence_RENAME_ME(double number, double seed)
		{
			return randomSequence(number, seed);
		}

		/// <summary>
		/// random-sequence() implementation;
		/// </summary>
		/// <param name="number"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		private XPathNodeIterator randomSequenceImpl(double number, int seed)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<randoms/>");

			if (seed == int.MinValue)
			{
			    seed += 1;
			}

		    Random rand = new Random(seed);

			//Negative number is bad idea - fallback to default
			if (number < 0)
			{
			    number = 1;
			}

		    //we limit number of generated numbers to int.MaxValue
			if (number > int.MaxValue)
			{
			    number = int.MaxValue;
			}

		    for (int i = 0; i < Convert.ToInt32(number); i++)
			{
				XmlElement elem = doc.CreateElement("random");
				elem.InnerText = rand.NextDouble().ToString();
				doc.DocumentElement.AppendChild(elem);
			}

			return doc.CreateNavigator().Select("/randoms/random");
		}
	}
}
