using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements additional functions in the 
	/// "http://gotdotnet.com/exslt/dates-and-times" namespace.    
	/// </summary>
	public class GdnDatesAndTimes : ExsltDatesAndTimes
	{
	    /// <summary>
		/// Implements the following function 
		///    string date2:avg(node-set)
		/// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-avg.xml  
		/// </summary>        
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
		public string Avg(XPathNodeIterator iterator)
		{
			TimeSpan sum = new TimeSpan(0, 0, 0, 0);
			int count = iterator.Count;
			if (count == 0)
			{
				return "";
			}
			try
			{
				while (iterator.MoveNext())
				{
					sum = XmlConvert.ToTimeSpan(iterator.Current.Value).Add(sum);
				}

			}
			catch (FormatException)
			{
				return "";
			}

			return Duration(sum.TotalSeconds / count);
		}

	    public string avg(XPathNodeIterator iterator) => Avg(iterator);

        /// <summary>
        /// Implements the following function 
        ///    string date2:min(node-set)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-min.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
        public string Min(XPathNodeIterator iterator)
		{

			TimeSpan min, t;

			if (iterator.Count == 0)
			{
				return "";
			}

			try
			{

				iterator.MoveNext();
				min = XmlConvert.ToTimeSpan(iterator.Current.Value);

				while (iterator.MoveNext())
				{
					t = XmlConvert.ToTimeSpan(iterator.Current.Value);
					min = (t < min) ? t : min;
				}

			}
			catch (FormatException)
			{
				return "";
			}

			return XmlConvert.ToString(min);
		}

	    public string min(XPathNodeIterator iterator) => Min(iterator);

        /// <summary>
        /// Implements the following function 
        ///    string date2:max(node-set)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-max.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>
        public string Max(XPathNodeIterator iterator)
		{

			TimeSpan max, t;

			if (iterator.Count == 0)
			{
				return "";
			}

			try
			{

				iterator.MoveNext();
				max = XmlConvert.ToTimeSpan(iterator.Current.Value);


				while (iterator.MoveNext())
				{
					t = XmlConvert.ToTimeSpan(iterator.Current.Value);
					max = (t > max) ? t : max;
				}

			}
			catch (FormatException)
			{
				return "";
			}

			return XmlConvert.ToString(max);
		}

	    public string max(XPathNodeIterator iterator) => Max(iterator);

        /// <summary>
        /// Implements the following function 
        ///    string date2:day-abbreviation(string)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-day-abbreviation.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
        /// <returns>The abbreviated current day name according to 
        /// specified culture or the empty string if the culture isn't 
        /// supported.</returns>
        public new string DayAbbreviation(string culture)
		{
			try
			{
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetAbbreviatedDayName(System.DateTime.Now.DayOfWeek);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public new string dayAbbreviation_RENAME_ME(string c)
	    {
	        return DayAbbreviation(c);
	    }

        /// <summary>
        /// Implements the following function 
        ///    string date2:day-abbreviation(string, string)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-day-abbreviation.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
        /// <returns>The abbreviated day name of the specified date according to 
        /// specified culture or the empty string if the input date is invalid or
        /// the culture isn't supported.</returns>
        public string DayAbbreviation(string d, string culture)
		{
			try
			{
				DateTz date = new DateTz(d);
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetAbbreviatedDayName(date.D.DayOfWeek);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public string dayAbbreviation_RENAME_ME(string d, string c)
	    {
	        return DayAbbreviation(d, c);
	    }

		/// <summary>
		/// Implements the following function 
		///    string date2:day-name(string, string?)
		/// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-day-name.xml
		/// </summary>        
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
		/// <returns>The day name of the specified date according to 
		/// specified culture or the empty string if the input date is invalid or
		/// the culture isn't supported.</returns>
		public string DayName(string d, string culture)
		{
			try
			{
				DateTz date = new DateTz(d);
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetDayName(date.D.DayOfWeek);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public string dayName_RENAME_ME(string d, string c)
	    {
	        return DayName(d, c);
	    }

        /// <summary>
        /// Implements the following function 
        ///    string date2:day-name(string, string?)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-day-name.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
        /// <returns>The day name of the current date according to 
        /// specified culture or the empty string if
        /// the culture isn't supported.</returns>
        public new string DayName(string culture)
		{
			try
			{
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetDayName(System.DateTime.Now.DayOfWeek);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public new string dayName_RENAME_ME(string c)
	    {
	        return DayName(c);
	    }

		/// <summary>
		/// Implements the following function 
		///    string date2:month-abbreviation(string)
		/// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-month-abbreviation.xml
		/// </summary>        
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
		/// <returns>The abbreviated current month name according to 
		/// specified culture or the empty string if the culture isn't 
		/// supported.</returns>
		public new string MonthAbbreviation(string culture)
		{
			try
			{
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetAbbreviatedMonthName(System.DateTime.Now.Month);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public new string monthAbbreviation_RENAME_ME(string c)
	    {
	        return MonthAbbreviation(c);
	    }

        /// <summary>
        /// Implements the following function 
        ///    string date2:month-abbreviation(string, string)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-month-abbreviation.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
        /// <returns>The abbreviated month name of the specified date according to 
        /// specified culture or the empty string if the input date is invalid or
        /// the culture isn't supported.</returns>
        public string MonthAbbreviation(string d, string culture)
		{
			try
			{
				DateTz date = new DateTz(d);
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetAbbreviatedMonthName(date.D.Month);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public string monthAbbreviation_RENAME_ME(string d, string c)
	    {
	        return MonthAbbreviation(d, c);
	    }

		/// <summary>
		/// Implements the following function 
		///    string date2:month-name(string, string?)
		/// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-month-name.xml
		/// </summary>        
		/// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
		/// <returns>The month name of the specified date according to 
		/// specified culture or the empty string if the input date is invalid or
		/// the culture isn't supported.</returns>
		public string MonthName(string d, string culture)
		{
			try
			{
				DateTz date = new DateTz(d);
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetMonthName(date.D.Month);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public string monthName_RENAME_ME(string d, string c)
	    {
	        return MonthName(d, c);
	    }

        /// <summary>
        /// Implements the following function 
        ///    string date2:month-name(string, string?)
        /// See http://www.xmland.net/exslt/doc/GDNDatesAndTimes-month-name.xml
        /// </summary>        
        /// <remarks>THIS FUNCTION IS NOT PART OF EXSLT!!!</remarks>    
        /// <returns>The month name of the current date according to 
        /// specified culture or the empty string if
        /// the culture isn't supported.</returns>
        public new string MonthName(string culture)
		{
			try
			{
				CultureInfo ci = new CultureInfo(culture);
				return ci.DateTimeFormat.GetMonthName(System.DateTime.Now.Month);
			}
			catch (Exception)
			{
				return "";
			}
		}

	    /// <summary>
	    /// This wrapper method will be renamed during custom build 
	    /// to provide conformant EXSLT function name.
	    /// </summary>
	    public new string monthName_RENAME_ME(string c)
	    {
	        return MonthName(c);
	    }
	}
}
