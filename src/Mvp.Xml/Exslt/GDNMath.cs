using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements addditional functions in the http://gotdotnet.com/exslt/math namespace.
	/// </summary>
	public class GdnMath
	{

		/// <summary>
		/// Implements the following function 
		///    number avg(node-set)
		/// </summary>
		/// <param name="iterator"></param>
		/// <returns>The average of all the value of all the nodes in the 
		/// node set</returns>
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
		public double Avg(XPathNodeIterator iterator)
		{

			double sum = 0;
			int count = iterator.Count;

			if (count == 0)
			{
				return double.NaN;
			}

			try
			{
				while (iterator.MoveNext())
				{
					sum += XmlConvert.ToDouble(iterator.Current.Value);
				}

			}
			catch (FormatException)
			{
				return double.NaN;
			}

			return sum / count;
		}

	    public double avg(XPathNodeIterator iterator) => Avg(iterator);
	}
}

