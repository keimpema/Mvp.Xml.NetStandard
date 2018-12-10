using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Text.RegularExpressions;

namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// This class implements the EXSLT functions in the http://exslt.org/dates-and-times namespace.
	/// </summary>
	public class ExsltDatesAndTimes
	{
		//private CultureInfo ci = new CultureInfo("en-US");

		private class ExsltDateTimeFactory
		{
			/// <summary>
			/// Parse a date and time for format-date() 
			/// </summary>
			/// <param name="d"></param>
			/// <returns></returns>
			public static ExsltDateTime ParseDateTime(string d)
			{
				// First try any of the classes in ParseDate
				try
				{
					return ParseDate(d);
				}
				catch (FormatException)
				{
				}

				try
				{
					TimeTz t = new TimeTz(d);
					return t;
				}
				catch (FormatException)
				{
				}

				try
				{
					MonthDay t = new MonthDay(d);
					return t;
				}
				catch (FormatException)
				{
				}

				try
				{
					Month t = new Month(d);
					return t;
				}
				catch (FormatException)
				{
				}

				// Finally day -- don't catch the exception
				{
					Day t = new Day(d);
					return t;
				}
			}

			/// <summary>
			/// Initialize the structure with the current date, time and timezone
			/// </summary>
			public static ExsltDateTime ParseDate(string d)
			{
				// Try each potential class, from most specific to least specific.

				// First DateTimeTZ
				try
				{
					DateTimeTz t = new DateTimeTz(d);
					return t;
				}
				catch (FormatException)
				{
				}

				// Next Date
				try
				{
					DateTz t = new DateTz(d);
					return t;
				}
				catch (FormatException)
				{
				}

				// Next YearMonth
				try
				{
					YearMonth t = new YearMonth(d);
					return t;
				}
				catch (FormatException)
				{
				}

				// Finally Year -- don't catch the exception for the last type
				{
					YearTz t = new YearTz(d);
					return t;
				}
			}
		}

		internal abstract class ExsltDateTime
		{
			public DateTime D;
			public TimeSpan Ts = new TimeSpan(TimeSpan.MinValue.Ticks);

			protected CultureInfo Ci = new CultureInfo("en-US");

			/// <summary>
			/// Initialize the structure with the current date, time and timezone
			/// </summary>
			public ExsltDateTime()
			{
				D = System.DateTime.Now;
			    Ts = TimeZoneInfo.Local.GetUtcOffset(System.DateTime.Now);
            }

			/// <summary>
			/// Initialize the DateTimeTZ structure with the date, time and timezone in the string.
			/// </summary>
			/// <param name="inS">An ISO8601 string</param>
			public ExsltDateTime(string inS)
			{
				string s = inS.Trim();
				D = System.DateTime.ParseExact(s, ExpectedFormats, Ci, DateTimeStyles.AdjustToUniversal);

				if (s.EndsWith("Z"))
				{
				    Ts = new TimeSpan(0, 0, 0);
				}
				else if (s.Length > 6)
				{
					string zoneStr = s.Substring(s.Length - 6, 6);
					if (zoneStr[3] == ':')
					{
						try
						{
							int hours = int.Parse(zoneStr.Substring(0, 3));
							int minutes = int.Parse(zoneStr.Substring(4, 2));
							if (hours < 0)
							{
							    minutes = -minutes;
							}

						    Ts = new TimeSpan(hours, minutes, 0);
							D = D.Add(Ts);	// Adjust to time zone relative time						
						}
						catch (Exception)
						{
						}
					}
				}
			}

			/// <summary>
			/// Exslt Copy constructor
			/// Initialize the structure with the date, time and timezone in the string.
			/// </summary>
			/// <param name="inS">An ExsltDateTime</param>
			public ExsltDateTime(ExsltDateTime inS)
			{
				D = inS.D;
				Ts = inS.Ts;
			}

			public bool HasTimeZone()
			{
				return !(TimeSpan.MinValue.Ticks == Ts.Ticks);
			}

			public DateTime ToUniversalTime()
			{
				if (!HasTimeZone())
				{
				    return D;
				}
				else
				{
				    return D.Subtract(Ts);
				}
			}

			/// <summary>
			/// Output as a standard (ISO8601) string
			/// </summary>
			/// <returns>the date and time as an ISO8601 string.  includes timezone</returns>
			public override string ToString()
			{
				return this.ToString(OutputFormat);
			}

			/// <summary>
			/// Output as a formatted string
			/// </summary>
			/// <returns>the date and time as a formatted string.  includes timezone</returns>
			public string ToString(string of)
			{
				StringBuilder retString = new StringBuilder("");

				retString.Append(D.ToString(of));
				retString.Append(GetTimeZone());

				return retString.ToString();
			}

			public string GetTimeZone()
			{
				StringBuilder retString = new StringBuilder();

				// if no ts specified, output without ts
				if (HasTimeZone())
				{
					if (0 == Ts.Hours && 0 == Ts.Minutes)
					{
					    retString.Append('Z');
					}
					else if (Ts.Hours >= 0 && Ts.Minutes >= 0)
					{
						retString.Append('+');
						retString.Append(Ts.Hours.ToString().PadLeft(2, '0'));
						retString.Append(':');
						retString.Append(Ts.Minutes.ToString().PadLeft(2, '0'));
					}
					else
					{
						retString.Append('-');
						retString.Append((-Ts.Hours).ToString().PadLeft(2, '0'));
						retString.Append(':');
						retString.Append((-Ts.Minutes).ToString().PadLeft(2, '0'));
					}
				}

				return retString.ToString();
			}

			public string GetGmtOffsetTimeZone()
			{
				StringBuilder retString = new StringBuilder();

				// if no ts specified, output without ts
				if (HasTimeZone())
				{
					retString.Append("GMT");
					if (0 != Ts.Hours || 0 != Ts.Minutes)
					{
						retString.Append(GetTimeZone());
					}
				}

				return retString.ToString();
			}

			public string Get822TimeZone()
			{
				StringBuilder retString = new StringBuilder();

				// if no ts specified, output without ts
				if (HasTimeZone())
				{
					if (0 == Ts.Hours && 0 == Ts.Minutes)
					{
					    retString.Append("GMT");
					}
					else if (Ts.Hours >= 0 && Ts.Minutes >= 0)
					{
						retString.Append('+');
						retString.Append(Ts.Hours.ToString().PadLeft(2, '0'));
						retString.Append(Ts.Minutes.ToString().PadLeft(2, '0'));
					}
					else
					{
						retString.Append('-');
						retString.Append((-Ts.Hours).ToString().PadLeft(2, '0'));
						retString.Append((-Ts.Minutes).ToString().PadLeft(2, '0'));
					}
				}

				return retString.ToString();
			}

			protected abstract string[] ExpectedFormats { get;}
			protected abstract string OutputFormat { get;}
		}

		internal class DateTimeTz : ExsltDateTime
		{
			public DateTimeTz() : base() { }
			public DateTimeTz(string inS) : base(inS) { }
			public DateTimeTz(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f"  
};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "yyyy-MM-dd\"T\"HH:mm:ss";
				}
			}
		}


		internal class DateTz : ExsltDateTime
		{
			public DateTz() : base() { }
			public DateTz(string inS) : base(inS) { }
			public DateTz(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"yyyy-MM-ddzzz",
											"yyyy-MM-ddZ",
											"yyyy-MM-dd"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "yyyy-MM-dd";
				}
			}
		}

		internal class TimeTz : ExsltDateTime
		{
			public TimeTz(string inS) : base(inS) { }
			public TimeTz() : base() { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"HH:mm:sszzz",
											"HH:mm:ssZ",
											"HH:mm:ss"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "HH:mm:ss";
				}
			}
		}

		internal class YearMonth : ExsltDateTime
		{
			public YearMonth() : base() { }
			public YearMonth(string inS) : base(inS) { }
			public YearMonth(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"yyyy-MM-dd",
											"yyyy-MM"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "yyyy-MM";
				}
			}
		}

		internal class YearTz : ExsltDateTime
		{
			public YearTz() : base() { }
			public YearTz(string inS) : base(inS) { }
			public YearTz(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"yyyy-MM-dd",
											"yyyy-MM",
											"yyyy"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "yyyy";
				}
			}
		}

		internal class Month : ExsltDateTime
		{
			public Month() : base() { }
			public Month(string inS) : base(inS) { }
			public Month(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
										 "yyyy-MM-dd\"T\"HH:mm:ssZ", 
										 "yyyy-MM-dd\"T\"HH:mm:ss",
                                         "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",   
										 "yyyy-MM-dd",
										 "yyyy-MM",
										 "--MM--"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "--MM--";
				}
			}
		}

		internal class Day : ExsltDateTime
		{
			public Day() : base() { }
			public Day(string inS) : base(inS) { }
			public Day(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"yyyy-MM-dd",
											"---dd",
											"--MM-dd"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "---dd";
				}
			}
		}

		internal class MonthDay : ExsltDateTime
		{
			public MonthDay() : base() { }
			public MonthDay(string inS) : base(inS) { }
			public MonthDay(ExsltDateTime inS) : base(inS) { }

			protected override string[] ExpectedFormats
			{
				get
				{
					return new string[] {"yyyy-MM-dd\"T\"HH:mm:sszzz", 
											"yyyy-MM-dd\"T\"HH:mm:ssZ", 
											"yyyy-MM-dd\"T\"HH:mm:ss",
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ffZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.ff",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fzzz",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.fZ",  
                                            "yyyy-MM-dd\"T\"HH:mm:ss.f",
											"yyyy-MM-dd",
											"--MM-dd"};
				}
			}

			protected override string OutputFormat
			{
				get
				{
					return "--MM-dd";
				}
			}
		}


		private string[] dayAbbrevs = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
		private string[] dayNames = {"Sunday", "Monday", "Tuesday", 
										"Wednesday", "Thursday", "Friday", "Saturday"};

		private string[] monthAbbrevs = {"Jan", "Feb", "Mar", "Apr", "May", "Jun",
											"Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
		private string[] monthNames = {"January", "February", "March", "April", "May", "June",
										  "July", "August", "September", 
										  "October", "November", "December"};


		/// <summary>
		/// Implements the following function
		///   string date:date-time()
		/// Output format is ISO 8601 (YYYY-MM-DDThh:mm:ss{Z | {+ | -}zz:zz}).
		/// YYYY - year with century
		/// MM - month in numbers with leading zero
		/// DD - day in numbers with leading zero
		/// T - the letter T
		/// hh - hours in numbers with leading zero (00-23).
		/// mm - minutes in numbers with leading zero (00-59).
		/// ss - seconds in numbers with leading zero (00-59).
		/// +/-zzzz - time zone expressed as hours and minutes from UTC.
		///		If UTC, then this is the letter Z
		///		If east of Greenwich, then -zz:zz (e.g. Pacific standard time is -08:00)
		///		If west of Greenwich, then +zz:zz (e.g. Tokyo is +09:00)
		/// </summary>
		/// <returns>The current time.</returns>		
		public string DateTime()
		{
			DateTimeTz d = new DateTimeTz();
			return DateTimeImpl(d);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dateTime_RENAME_ME()
		{
			return DateTime();
		}

		/// <summary>
		/// Implements the following function
		///   string date:date-time()
		/// </summary>
		/// <returns>The current date and time or the empty string if the 
		/// date is invalid </returns>        
		public string DateTime(string s)
		{
			try
			{
				DateTimeTz d = new DateTimeTz(s);
				return DateTimeImpl(d);
			}
			catch (FormatException)
			{
				return "";
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dateTime_RENAME_ME(string d)
		{
			return DateTime(d);
		}

		/// <summary>
		/// Internal function to format the date based on a date, rather than a string
		/// </summary>
		/// <returns>The formtted date and time as a ISO8601 string</returns>        

		internal string DateTimeImpl(DateTimeTz dtz)
		{
			return dtz.ToString();
		}

		/// <summary>
		/// Implements the following function
		///   string date:date()
		/// </summary>
		/// <returns>The current date</returns>        
		public string Date()
		{
			DateTz dtz = new DateTz();
			return dtz.ToString();
		}

	    public string date() => Date();

        /// <summary>
        /// Implements the following function
        ///   string date:date(string)
        /// </summary>
        /// <returns>The date part of the specified date or the empty string if the 
        /// date is invalid</returns>        
        public string Date(string d)
		{
			try
			{
				DateTz dtz = new DateTz(d);
				return dtz.ToString();
			}
			catch (FormatException)
			{
				return "";
			}
		}

	    public string date(string d) => Date(d);

        /// <summary>
        /// Implements the following function
        ///   string date:time()
        /// </summary>
        /// <returns>The current time</returns>        
        public string Time()
		{
			TimeTz t = new TimeTz();
			return t.ToString();
		}

	    public string time() => Time();

        /// <summary>
        /// Implements the following function
        ///   string date:time(string)
        /// </summary>
        /// <returns>The time part of the specified date or the empty string if the 
        /// date is invalid</returns>        
        public string Time(string d)
		{
			try
			{
				TimeTz t = new TimeTz(d);
				return t.ToString();
			}
			catch (FormatException)
			{
				return "";
			}
		}

	    public string time(string d) => Time(d);

        /// <summary>
        /// Implements the following function
        ///   number date:year()
        /// </summary>
        /// <returns>The current year</returns>        
        public double Year()
		{
			return System.DateTime.Now.Year;
		}

	    public double year() => Year();

        /// <summary>
        /// Implements the following function
        ///   number date:year(string)
        /// </summary>
        /// <returns>The year part of the specified date or the empty string if the 
        /// date is invalid</returns>
        /// <remarks>Does not support dates in the format of the xs:yearMonth or 
        /// xs:gYear types</remarks>        
        public double Year(string d)
		{
			try
			{
				YearTz date = new YearTz(d);
				return date.D.Year;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

	    public double year(string d) => Year(d);

        /// <summary>
        /// Helper method for calculating whether a year is a leap year. Algorithm 
        /// obtained from http://mindprod.com/jglossleapyear.html
        /// </summary>        
        private static bool IsLeapYear(int year)
		{
			try
			{
				return CultureInfo.CurrentCulture.Calendar.IsLeapYear(year);
			}
			catch
			{
				return false;
			}
		}


		/// <summary>
		/// Implements the following function
		///   boolean date:leap-year()
		/// </summary>
		/// <returns>True if the current year is a leap year.</returns>        
		public bool LeapYear()
		{
			return IsLeapYear((int)Year());
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public bool leapYear_RENAME_ME()
		{
			return LeapYear();
		}

		/// <summary>
		/// Implements the following function
		///   boolean date:leap-year(string)
		/// </summary>
		/// <returns>True if the specified year is a leap year</returns>
		/// <remarks>Note that the spec says we should return NaN for a badly formatted input
		/// string.  This is impossible; we return false for a badly formatted input string.
		/// </remarks>        
		public bool LeapYear(string d)
		{
			double y = Year(d);

			if (y == double.NaN)
			{
			    return false;
			}
			else
			{
			    return IsLeapYear((int)y);
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public bool leapYear_RENAME_ME(string d)
		{
			return LeapYear(d);
		}

		/// <summary>
		/// Implements the following function
		///   number date:month-in-year()
		/// </summary>
		/// <returns>The current month</returns>        
		public double MonthInYear()
		{
			return System.DateTime.Now.Month;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double monthInYear_RENAME_ME()
		{
			return MonthInYear();
		}

		/// <summary>
		/// Implements the following function
		///   number date:month-in-year(string)
		/// </summary>
		/// <returns>The month part of the specified date or the empty string if the 
		/// date is invalid</returns>
		/// <remarks>Does not support dates in the format of xs:gYear</remarks>        
		public double MonthInYear(string d)
		{
			try
			{
				Month date = new Month(d);
				return date.D.Month;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double monthInYear_RENAME_ME(string d)
		{
			return MonthInYear(d);
		}

		/// <summary>
		/// Helper funcitno to calculate the week number
		/// </summary>
		/// <returns>
		/// Returns the week in the year.  Obeys EXSLT spec, which specifies that counting follows 
		/// ISO 8601: week 1 in a year is the week containing the first Thursday of the year, with 
		/// new weeks beginning on a Monday
		/// </returns>
		/// <param name="d">The date for which we want to find the week</param>
		private double WeekInYear(DateTime d)
		{
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(d,
										  System.Globalization.CalendarWeekRule.FirstFourDayWeek,
										  System.DayOfWeek.Monday);
		}

		/// <summary>
		/// Implements the following function
		///   number date:week-in-year()
		/// </summary>
		/// <returns>
		/// The current week. Obeys EXSLT spec, which specifies that counting follows 
		/// ISO 8601: week 1 in a year is the week containing the first Thursday of the year, with 
		/// new weeks beginning on a Monday
		/// </returns>        
		public double WeekInYear()
		{
			return this.WeekInYear(System.DateTime.Now);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double weekInYear_RENAME_ME()
		{
			return WeekInYear();
		}

		/// <summary>
		/// Implements the following function
		///   number date:week-in-year(string)
		/// </summary>
		/// <returns>The week part of the specified date or the empty string if the 
		/// date is invalid</returns>
		/// <remarks>Does not support dates in the format of the xs:yearMonth or 
		/// xs:gYear types. This method uses the Calendar.GetWeekOfYear() method 
		/// with the CalendarWeekRule and FirstDayOfWeek of the current culture.
		/// THE RESULTS OF CALLING THIS FUNCTION VARIES ACROSS CULTURES</remarks>        
		public double WeekInYear(string d)
		{
			try
			{
				DateTz dtz = new DateTz(d);
				return WeekInYear(dtz.D);
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double weekInYear_RENAME_ME(string d)
		{
			return WeekInYear(d);
		}

		/// <summary>
		/// Helper method. 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		private double WeekInMonth(DateTime d)
		{
			//
			// mon = 1
			// tue = 2
			// sun = 7
			// week = ceil(((date-day) / 7)) + 1

			double offset = (d.DayOfWeek == DayOfWeek.Sunday) ? 7 : (double)d.DayOfWeek;
			return System.Math.Ceiling((d.Day - offset) / 7) + 1;
		}

		/// <summary>
		/// Implements the following function
		///   number date:week-in-month()
		/// </summary>
		/// <remarks>
		/// The current week in month as a number.  For the purposes of numbering, the first 
		/// day of the month is in week 1 and new weeks begin on a Monday (so the first and 
		/// last weeks in a month will often have less than 7 days in them). 
		/// </remarks>        
		public double WeekInMonth()
		{
			return this.WeekInMonth(System.DateTime.Now);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double weekInMonth_RENAME_ME()
		{
			return WeekInMonth();
		}

		/// <summary>
		/// Implements the following function
		///   number date:week-in-month(string)
		/// </summary>
		/// <returns>The week in month of the specified date or NaN if the 
		/// date is invalid</returns>
		/// <remarks>
		/// The current week in month as a number.  For the purposes of numbering, the first 
		/// day of the month is in week 1 and new weeks begin on a Monday (so the first and 
		/// last weeks in a month will often have less than 7 days in them). 
		/// </remarks>        
		public double WeekInMonth(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return this.WeekInMonth(date.D);
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double weekInMonth_RENAME_ME(string d)
		{
			return WeekInMonth(d);
		}


		/// <summary>
		/// Implements the following function
		///   number date:day-in-year()
		/// </summary>
		/// <returns>The current day. </returns>        
		public double DayInYear()
		{
			return System.DateTime.Now.DayOfYear;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInYear_RENAME_ME()
		{
			return DayInYear();
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-in-year(string)
		/// </summary>
		/// <returns>
		/// The day part of the specified date or NaN if the date is invalid
		/// </returns>        
		public double DayInYear(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return date.D.DayOfYear;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInYear_RENAME_ME(string d)
		{
			return DayInYear(d);
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-in-week()
		/// </summary>
		/// <returns>The current day in the week. 1=Sunday, 2=Monday,...,7=Saturday</returns>        
		public double DayInWeek()
		{
			return ((int)System.DateTime.Now.DayOfWeek) + 1;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInWeek_RENAME_ME()
		{
			return DayInWeek();
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-in-week(string)
		/// </summary>
		/// <returns>The day in the week of the specified date or NaN if the 
		/// date is invalid. The current day in the week. 1=Sunday, 2=Monday,...,7=Saturday
		/// </returns>        
		public double DayInWeek(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return ((int)date.D.DayOfWeek) + 1;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInWeek_RENAME_ME(string d)
		{
			return DayInWeek(d);
		}


		/// <summary>
		/// Implements the following function
		///   number date:day-in-month()
		/// </summary>
		/// <returns>The current day. </returns>        
		public double DayInMonth()
		{
			return System.DateTime.Now.Day;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInMonth_RENAME_ME()
		{
			return DayInMonth();
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-in-month(string)
		/// </summary>
		/// <returns>The day part of the specified date or the empty string if the 
		/// date is invalid</returns>
		public double DayInMonth(string d)
		{
			try
			{
				Day date = new Day(d);
				return date.D.Day;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayInMonth_RENAME_ME(string d)
		{
			return DayInMonth(d);
		}

		/// <summary>
		/// Helper method.
		/// </summary>
		/// <param name="day"></param>
		/// <returns></returns>
		private double DayOfWeekInMonth(int day)
		{
			// day of week in month = floor(((date-1) / 7)) + 1
			return ((day - 1) / 7) + 1;
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-of-week-in-month()
		/// </summary>
		/// <returns>The current day of week in the month as a number. For instance 
		/// the third Friday of the month returns 3</returns>        
		public double DayOfWeekInMonth()
		{
			return this.DayOfWeekInMonth(System.DateTime.Now.Day);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayOfWeekInMonth_RENAME_ME()
		{
			return DayOfWeekInMonth();
		}

		/// <summary>
		/// Implements the following function
		///   number date:day-of-week-in-month(string)
		/// </summary>
		/// <returns>The day part of the specified date or NaN if the 
		/// date is invalid</returns>        
		public double DayOfWeekInMonth(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return this.DayOfWeekInMonth(date.D.Day);
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double dayOfWeekInMonth_RENAME_ME(string d)
		{
			return DayOfWeekInMonth(d);
		}

		/// <summary>
		/// Implements the following function
		///   number date:hour-in-day()
		/// </summary>
		/// <returns>The current hour of the day as a number.</returns>        
		public double HourInDay()
		{
			return System.DateTime.Now.Hour;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double hourInDay_RENAME_ME()
		{
			return HourInDay();
		}

		/// <summary>
		/// Implements the following function
		///   number date:hour-in-day(string)
		/// </summary>
		/// <returns>The current hour of the specified time or NaN if the 
		/// date is invalid</returns>        
		public double HourInDay(string d)
		{
			try
			{
				TimeTz date = new TimeTz(d);
				return date.D.Hour;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double hourInDay_RENAME_ME(string d)
		{
			return HourInDay(d);
		}

		/// <summary>
		/// Implements the following function
		///   number date:minute-in-hour()
		/// </summary>
		/// <returns>The minute of the current hour as a number. </returns>        
		public double MinuteInHour()
		{
			return System.DateTime.Now.Minute;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double minuteInHour_RENAME_ME()
		{
			return MinuteInHour();
		}

		/// <summary>
		/// Implements the following function
		///   number date:minute-in-hour(string)
		/// </summary>
		/// <returns>The minute of the hour of the specified time or NaN if the 
		/// date is invalid</returns>        
		public double MinuteInHour(string d)
		{
			try
			{
				TimeTz date = new TimeTz(d);
				return date.D.Minute;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double minuteInHour_RENAME_ME(string d)
		{
			return MinuteInHour(d);
		}

		/// <summary>
		/// Implements the following function
		///   number date:second-in-minute()
		/// </summary>
		/// <returns>The seconds of the current minute as a number. </returns>        
		public double SecondInMinute()
		{
			return System.DateTime.Now.Second;
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double secondInMinute_RENAME_ME()
		{
			return SecondInMinute();
		}

		/// <summary>
		/// Implements the following function
		///   number date:second-in-minute(string)
		/// </summary>
		/// <returns>The seconds of the minute of the specified time or NaN if the 
		/// date is invalid</returns>        
		public double SecondInMinute(string d)
		{
			try
			{
				TimeTz date = new TimeTz(d);
				return date.D.Second;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public double secondInMinute_RENAME_ME(string d)
		{
			return SecondInMinute(d);
		}

		/// <summary>
		/// Helper function for 
		///   string date:day-name()
		/// </summary>
		/// <returns>The Engish name of the current day</returns>        
		private string DayName(int dow)
		{
			if (dow < 0 || dow >= dayNames.Length)
			{
			    return string.Empty;
			}

		    return dayNames[dow];
		}

		/// <summary>
		/// Implements the following function
		///   string date:day-name()
		/// </summary>
		/// <returns>The Engish name of the current day</returns>        
		public string DayName()
		{
			return DayName((int)System.DateTime.Now.DayOfWeek);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dayName_RENAME_ME()
		{
			return DayName();
		}

		/// <summary>
		/// Implements the following function
		///   string date:day-name(string)
		/// </summary>
		/// <returns>The English name of the day of the specified date or the empty string if the 
		/// date is invalid</returns>        
		public string DayName(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return DayName((int)date.D.DayOfWeek);
			}
			catch (FormatException)
			{
				return "";
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dayName_RENAME_ME(string d)
		{
			return DayName(d);
		}

		/// <summary>
		/// Helper function for 
		///   string date:day-abbreviation()
		/// </summary>
		/// <returns>The abbreviated English name of the current day</returns>        
		private string DayAbbreviation(int dow)
		{
			if (dow < 0 || dow >= dayAbbrevs.Length)
			{
			    return string.Empty;
			}

		    return dayAbbrevs[dow];
		}


		/// <summary>
		/// Implements the following function
		///   string date:day-abbreviation()
		/// </summary>
		/// <returns>The abbreviated English name of the current day</returns>        
		public string DayAbbreviation()
		{
			return DayAbbreviation((int)System.DateTime.Now.DayOfWeek);
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dayAbbreviation_RENAME_ME()
		{
			return DayAbbreviation();
		}

		/// <summary>
		/// Implements the following function
		///   string date:day-abbreviation(string)
		/// </summary>
		/// <returns>The abbreviated English name of the day of the specified date or the 
		/// empty string if the input date is invalid</returns>        
		public string DayAbbreviation(string d)
		{
			try
			{
				DateTz date = new DateTz(d);
				return DayAbbreviation((int)date.D.DayOfWeek);
			}
			catch (FormatException)
			{
				return "";
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string dayAbbreviation_RENAME_ME(string d)
		{
			return DayAbbreviation(d);
		}

		/// <summary>
		/// Helper Function for 
		///   string date:month-name()
		/// </summary>
		/// <returns>The name of the current month</returns>        
		private string MonthName(int month)
		{
			if (month < 1 || month > monthNames.Length)
			{
			    return string.Empty;
			}

		    return monthNames[month - 1];
		}

		/// <summary>
		/// Implements the following function
		///   string date:month-name()
		/// </summary>
		/// <returns>The name of the current month</returns>        
		public string MonthName()
		{
			return MonthName((int)MonthInYear());
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string monthName_RENAME_ME()
		{
			return MonthName();
		}

		/// <summary>
		/// Implements the following function
		///   string date:month-name(string)
		/// </summary>
		/// <returns>The name of the month of the specified date or the empty string if the 
		/// date is invalid</returns>
		/// <remarks>Does not support dates in the format of xs:gYear types</remarks>        
		public string MonthName(string d)
		{
			double month = MonthInYear(d);
			if (month == double.NaN)
			{
			    return "";
			}
			else
			{
			    return MonthName((int)month);
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string monthName_RENAME_ME(string d)
		{
			return MonthName(d);
		}

		/// <summary>
		/// Helper function for 
		///   string date:month-abbreviation()
		/// </summary>
		/// <returns>The abbreviated name of the current month</returns>        
		private string MonthAbbreviation(int month)
		{
			if (month < 1 || month > monthAbbrevs.Length)
			{
			    return string.Empty;
			}

		    return monthAbbrevs[month - 1];
		}

		/// <summary>
		/// Implements the following function
		///   string date:month-abbreviation()
		/// </summary>
		/// <returns>The abbreviated name of the current month</returns>        
		public string MonthAbbreviation()
		{
			return MonthAbbreviation((int)MonthInYear());
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string monthAbbreviation_RENAME_ME()
		{
			return MonthAbbreviation();
		}

		/// <summary>
		/// Implements the following function
		///   string date:month-abbreviation(string)
		/// </summary>
		/// <returns>The abbreviated name of the month of the specified date or the empty string if the 
		/// date is invalid</returns>
		/// <remarks>Does not support dates in the format of the xs:yearMonth or 
		/// xs:gYear types</remarks>        
		public string MonthAbbreviation(string d)
		{
			double month = MonthInYear(d);
			if (month == double.NaN)
			{
			    return "";
			}
			else
			{
			    return MonthAbbreviation((int)month);
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string monthAbbreviation_RENAME_ME(string d)
		{
			return MonthAbbreviation(d);
		}

		/// <summary>
		/// Implements the following function
		///   string date:format-date(string, string)
		/// </summary>
		/// <param name="d">The date to format</param>
		/// <param name="format">One of the format strings understood by the 
		/// Java 1.1 SimpleDateFormat method:
		/// 
		///  Symbol   Meaning                 Presentation        Example
		///------   -------                 ------------        -------
		///G        era designator          (Text)              AD
		///y        year                    (Number)            1996
		///M        month in year           (Text &amp; Number)     July &amp; 07
		///d        day in month            (Number)            10
		///h        hour in am/pm (1~12)    (Number)            12
		///H        hour in day (0~23)      (Number)            0
		///m        minute in hour          (Number)            30
		///s        second in minute        (Number)            55
		///S        millisecond             (Number)            978
		///E        day in week             (Text)              Tuesday
		///D        day in year             (Number)            189
		///F        day of week in month    (Number)            2 (2nd Wed in July)
		///w        week in year            (Number)            27
		///W        week in month           (Number)            2
		///a        am/pm marker            (Text)              PM
		///k        hour in day (1~24)      (Number)            24
		///K        hour in am/pm (0~11)    (Number)            0
		///z        time zone               (Text)              Pacific Standard Time
		///'        escape for text         (Delimiter)
		///''       single quote            (Literal)           '
		///</param>
		/// <returns>The formated date</returns>        
		public string FormatDate(string d, string format)
		{
			try
			{
				ExsltDateTime oDate = ExsltDateTimeFactory.ParseDateTime(d);
				StringBuilder retString = new StringBuilder("");

				for (int i = 0; i < format.Length; )
				{
					int s = i;
					switch (format[i])
					{
						case 'G'://        era designator          (Text)              AD
							while (i < format.Length && format[i] == 'G') { i++; }

							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								if (oDate.D.Year < 0)
								{
									retString.Append("BC");
								}
								else
								{
									retString.Append("AD");
								}
							}
							break;

						case 'y'://        year                    (Number)            1996
							while (i < format.Length && format[i] == 'y') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								if (i - s == 2)
								{
									retString.Append((oDate.D.Year % 100).ToString().PadLeft(i - s, '0'));

								}
								else
								{
									retString.Append(oDate.D.Year.ToString().PadLeft(i - s, '0'));
								}
							}
							break;
						case 'M'://        month in year           (Text &amp; Number)     July &amp; 07
							while (i < format.Length && format[i] == 'M') { i++; }

							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(Month)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(MonthDay)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								if (i - s <= 2)
								{
								    retString.Append(oDate.D.Month.ToString().PadLeft(i - s, '0'));
								}
								else if (i - s == 3)
								{
								    retString.Append(MonthAbbreviation(oDate.D.Month));
								}
								else
								{
								    retString.Append(MonthName(oDate.D.Month));
								}
							}
							break;
						case 'd'://        day in month            (Number)            10
							while (i < format.Length && format[i] == 'd') { i++; }

							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(MonthDay)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(Day)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.Day.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'h'://        hour in am/pm (1~12)    (Number)            12
							while (i < format.Length && format[i] == 'h') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								int hour = oDate.D.Hour % 12;
								if (0 == hour)
								{
								    hour = 12;
								}

							    retString.Append(hour.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'H'://        hour in day (0~23)      (Number)            0
							while (i < format.Length && format[i] == 'H') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.Hour.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'm'://        minute in hour          (Number)            30
							while (i < format.Length && format[i] == 'm') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.Minute.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 's'://        second in minute        (Number)            55
							while (i < format.Length && format[i] == 's') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.Second.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'S'://        millisecond             (Number)            978
							while (i < format.Length && format[i] == 'S') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.Millisecond.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'E'://        day in week             (Text)              Tuesday
							while (i < format.Length && format[i] == 'E') { i++; }

							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								if (i - s <= 3)
								{
									retString.Append(DayAbbreviation((int)oDate.D.DayOfWeek));
								}
								else
								{
									retString.Append(DayName((int)oDate.D.DayOfWeek));
								}
							}
							break;
						case 'D'://        day in year             (Number)            189
							while (i < format.Length && format[i] == 'D') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(oDate.D.DayOfYear.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'F'://        day of week in month    (Number)            2 (2nd Wed in July)
							while (i < format.Length && format[i] == 'F') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(MonthDay)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(Day)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(DayOfWeekInMonth(oDate.D.Day).ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'w'://        week in year            (Number)            27
							while (i < format.Length && format[i] == 'w') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(WeekInYear(oDate.D));
							}
							break;
						case 'W'://        week in month           (Number)            2
							while (i < format.Length && format[i] == 'W') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								retString.Append(WeekInMonth(oDate.D));
							}
							break;
						case 'a'://        am/pm marker            (Text)              PM
							while (i < format.Length && format[i] == 'a') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								if (oDate.D.Hour < 12)
								{
								    retString.Append("AM");
								}
								else
								{
								    retString.Append("PM");
								}
							}
							break;
						case 'k'://        hour in day (1~24)      (Number)            24
							while (i < format.Length && format[i] == 'k') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								int hour = oDate.D.Hour + 1;
								retString.Append(hour.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'K'://        hour in am/pm (0~11)    (Number)            0
							while (i < format.Length && format[i] == 'K') { i++; }
							if (object.ReferenceEquals(oDate.GetType(), typeof(DateTimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(DateTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearMonth)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(TimeTz)) ||
								object.ReferenceEquals(oDate.GetType(), typeof(YearTz)))
							{
								int hour = oDate.D.Hour % 12;
								retString.Append(hour.ToString().PadLeft(i - s, '0'));
							}
							break;
						case 'z'://        time zone               (Text)              Pacific Standard Time
							while (i < format.Length && format[i] == 'z') { i++; }
							//
							// BUGBUG: Need to convert to full timezone names or timezone abbrevs
							// if they are available.  Now cheating by using GMT offsets.
							retString.Append(oDate.GetGmtOffsetTimeZone());
							break;
						case 'Z'://			rfc 822 time zone
							while (i < format.Length && format[i] == 'Z') { i++; }
							retString.Append(oDate.Get822TimeZone());
							break;
						case '\''://        escape for text         (Delimiter)
							if (i < format.Length && format[i + 1] == '\'')
							{
								i++;
								while (i < format.Length && format[i] == '\'') { i++; }
								retString.Append("'");
							}
							else
							{
								i++;
								while (i < format.Length && format[i] != '\'' && i <= format.Length) { retString.Append(format.Substring(i++, 1)); }
								if (i >= format.Length)
								{
								    return "";
								}

							    i++;
							}
							break;
						default:
							retString.Append(format[i]);
							i++;
							break;
					}
				}

				return retString.ToString();
			}


			catch (FormatException)
			{
				return "";
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string formatDate_RENAME_ME(string d, string format)
		{
			return FormatDate(d, format);
		}


		/// <summary>
		/// Implements the following function
		///   string date:parse-date(string, string)
		/// BUGBUG: should use Java formatting strings, not Windows.
		/// </summary>
		/// <param name="d">The date to parse</param>
		/// <param name="format">One of the format strings understood by the 
		/// DateTime.ToString(string) method.</param>
		/// <returns>The parsed date</returns>        
		public string ParseDate(string d, string format)
		{
			try
			{
				DateTime date = System.DateTime.ParseExact(d, format, CultureInfo.CurrentCulture);
				return XmlConvert.ToString(date, XmlDateTimeSerializationMode.RoundtripKind);
			}
			catch (FormatException)
			{
				return "";
			}
		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string parseDate_RENAME_ME(string d, string format)
		{
			return ParseDate(d, format);
		}

		/// <summary>
		/// Implements the following function 
		///    string:date:difference(string, string)
		/// </summary>
		/// <param name="start">The start date</param>
		/// <param name="end">The end date</param>
		/// <returns>A positive difference if start is before end otherwise a negative
		/// difference. The difference is in the ISO 8601 date difference format as either
		/// [-]P[yY][mM]
		/// or
		/// [-]P[dD][T][hH][mM][sS]
		/// P means a difference and is required
		/// At least one of Y M D H M S is required
		/// If a higher order component is 0, it is suppressed (for example, if there are 0 years and
		/// 1 month, then Y is suppressed.  If there are 1 years and 0 months, M is not suppressed)
		/// If the input format is yyyy-MM or yyyy, then the output is [-]P[yY][mM]
		/// If the input format includes days, hours, minutes or seconds, then the output is
		/// [-]P[dD][T][hH][mM][sS].
		/// If there H M and S are all 0, the T is suppressed.
		/// </returns>        
		public string Difference(string start, string end)
		{
			try
			{
				ExsltDateTime startdate = ExsltDateTimeFactory.ParseDate(start);
				ExsltDateTime enddate = ExsltDateTimeFactory.ParseDate(end);

				// The rules are pretty tricky.  basically, interpret both strings as the least-
				// specific format
				if (object.ReferenceEquals(startdate.GetType(), typeof(YearTz)) ||
					object.ReferenceEquals(enddate.GetType(), typeof(YearTz)))
				{
					StringBuilder retString = new StringBuilder("");

					int yearDiff = enddate.D.Year - startdate.D.Year;

					if (yearDiff < 0)
					{
					    retString.Append('-');
					}

				    retString.Append('P');
					retString.Append(Math.Abs(yearDiff));
					retString.Append('Y');

					return retString.ToString();
				}
				else if (object.ReferenceEquals(startdate.GetType(), typeof(YearMonth)) ||
					object.ReferenceEquals(enddate.GetType(), typeof(YearMonth)))
				{
					StringBuilder retString = new StringBuilder("");

					int yearDiff = enddate.D.Year - startdate.D.Year;
					int monthDiff = enddate.D.Month - startdate.D.Month;

					// Borrow from the year if necessary
					if ((yearDiff > 0) && (Math.Sign(monthDiff) == -1))
					{
						yearDiff--;
						monthDiff += 12;
					}
					else if ((yearDiff < 0) && (Math.Sign(monthDiff) == 1))
					{
						yearDiff++;
						monthDiff -= 12;
					}


					if ((yearDiff < 0) || ((yearDiff == 0) && (monthDiff < 0)))
					{
						retString.Append('-');
					}
					retString.Append('P');
					if (yearDiff != 0)
					{
						retString.Append(Math.Abs(yearDiff));
						retString.Append('Y');
					}
					retString.Append(Math.Abs(monthDiff));
					retString.Append('M');

					return retString.ToString();
				}
				else
				{
					// Simulate casting to the most truncated format.  i.e. if one 
					// Arg is DateTZ and the other is DateTimeTZ, get rid of the time
					// for both.
					if (object.ReferenceEquals(startdate.GetType(), typeof(DateTz)) ||
						object.ReferenceEquals(enddate.GetType(), typeof(DateTz)))
					{
						startdate = new DateTz(startdate.D.ToString("yyyy-MM-dd"));
						enddate = new DateTz(enddate.D.ToString("yyyy-MM-dd"));
					}

					TimeSpan ts = enddate.D.Subtract(startdate.D);
					return XmlConvert.ToString(ts);
				}
			}
			catch (FormatException)
			{
				return "";
			}
		}

	    public string difference(string start, string end) => Difference(start, end);

        /// <summary>
        /// Implements the following function
        ///    date:add(string, string)
        /// </summary>
        /// <param name="datetime">An ISO8601 date/time</param>
        /// <param name="duration">the duration to add</param>
        /// <returns>The new time</returns>        
        public string Add(string datetime, string duration)
		{
			try
			{
				ExsltDateTime date = ExsltDateTimeFactory.ParseDate(datetime);
				//TimeSpan timespan = System.Xml.XmlConvert.ToTimeSpan(duration); 

				Regex durationRe = new Regex(
					@"^(-)?" +				// May begin with a - sign
					@"P" +					// Must contain P as first or 2nd char
					@"(?=\d+|(?:T\d+))" +		// Must contain at least one digit after P or after PT
					@"(?:(\d+)Y)?" +		// May contain digits plus Y for year
					@"(?:(\d+)M)?" +		// May contain digits plus M for month
					@"(?:(\d+)D)?" +		// May contain digits plus D for day
					@"(?=T\d+)?" +			// If there is a T here, must be digits afterwards
					@"T?" +					// May contain a T
					@"(?:(\d+)H)?" +		// May contain digits plus H for hours
					@"(?:(\d+)M)?" +		// May contain digits plus M for minutes
					@"(?:(\d+)S)?" +		// May contain digits plus S for seconds
					@"$",
					RegexOptions.IgnoreCase | RegexOptions.Singleline
					);

				Match m = durationRe.Match(duration);

				int negation = 1, years = 0, months = 0, days = 0,
					hours = 0, minutes = 0, seconds = 0;

				if (m.Success)
				{
					//date.d = date.d.Add(timespan);
					// According to the XML datetime spec at 
					// http://www.w3.org/TR/xmlschema-2/#adding-durations-to-dateTimes, 
					// we need to first add the year/month part, then we can add the 
					// day/hour/minute/second part

					if (CultureInfo.InvariantCulture.CompareInfo.Compare(m.Groups[1].Value, "-") == 0)
					{
					    negation = -1;
					}

				    if (m.Groups[2].Length > 0)
				    {
				        years = negation * int.Parse(m.Groups[2].Value);
				    }

				    if (m.Groups[3].Length > 0)
				    {
				        months = negation * int.Parse(m.Groups[3].Value);
				    }

				    if (m.Groups[4].Length > 0)
				    {
				        days = negation * int.Parse(m.Groups[4].Value);
				    }

				    if (m.Groups[5].Length > 0)
				    {
				        hours = negation * int.Parse(m.Groups[5].Value);
				    }

				    if (m.Groups[6].Length > 0)
				    {
				        minutes = negation * int.Parse(m.Groups[6].Value);
				    }

				    if (m.Groups[7].Length > 0)
				    {
				        seconds = negation * int.Parse(m.Groups[7].Value);
				    }

				    date.D = date.D.AddYears(years);
					date.D = date.D.AddMonths(months);
					date.D = date.D.AddDays(days);
					date.D = date.D.AddHours(hours);
					date.D = date.D.AddMinutes(minutes);
					date.D = date.D.AddSeconds(seconds);

					// we should return the same format as passed in

					// return date.ToString("yyyy-MM-dd\"T\"HH:mm:ss");			
					return date.ToString();
				}
				else
				{
					return "";
				}
			}
			catch (FormatException)
			{
				return "";
			}
		}

	    public string add(string datetime, string duration) => Add(datetime, duration);


        /// <summary>
        /// Implements the following function
        ///    string:date:add-duration(string, string)
        /// </summary>
        /// <param name="duration1">Initial duration</param>
        /// <param name="duration2">the duration to add</param>
        /// <returns>The new time</returns>        
        public string AddDuration(string duration1, string duration2)
		{
			try
			{
				TimeSpan timespan1 = XmlConvert.ToTimeSpan(duration1);
				TimeSpan timespan2 = XmlConvert.ToTimeSpan(duration2);
				return XmlConvert.ToString(timespan1.Add(timespan2));
			}
			catch (FormatException)
			{
				return "";
			}

		}

		/// <summary>
		/// This wrapper method will be renamed during custom build 
		/// to provide conformant EXSLT function name.
		/// </summary>
		public string addDuration_RENAME_ME(string duration1, string duration2)
		{
			return AddDuration(duration1, duration2);
		}

		/// <summary>
		/// Helper method for date:seconds() that takes an ExsltDateTime. 
		/// </summary>
		/// <param name="d"></param>
		/// <returns>difference in seconds between the specified date and the
		/// epoch (1970-01-01T00:00:00Z)</returns>
		private double Seconds(ExsltDateTime d)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, CultureInfo.InvariantCulture.Calendar);
			return d.ToUniversalTime().Subtract(epoch).TotalSeconds;
		}

		/// <summary>
		/// Implements the following function
		///		number date:seconds()
		/// </summary>
		/// <returns>The amount of seconds since the epoch (1970-01-01T00:00:00Z)</returns>        
		public double Seconds()
		{
			return Seconds(new DateTimeTz());
		}

	    public double seconds() => Seconds();

        /// <summary>
        /// Implements the following function
        ///		number date:seconds(string)
        /// </summary>
        /// <returns>If date passed in, the amount of seconds between the specified date and the 
        /// epoch (1970-01-01T00:00:00Z).  If timespan passed in, returns the number of seconds
        /// in the timespan.</returns>
        public double Seconds(string datetime)
		{
			try
			{
				return Seconds(ExsltDateTimeFactory.ParseDate(datetime));
			}
			catch (FormatException) { ; } //might be a duration

			try
			{
				TimeSpan duration = XmlConvert.ToTimeSpan(datetime);
				return duration.TotalSeconds;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}

	    public double seconds(string datetime) => Seconds(datetime);

        /// <summary>
        /// Implements the following function 
        ///		string date:sum(node-set)
        /// </summary>
        /// <param name="iterator">Nodeset of timespans</param>
        /// <returns>The sum of the timespans within a node set.</returns>        
        public string Sum(XPathNodeIterator iterator)
		{

			TimeSpan sum = new TimeSpan(0, 0, 0, 0);

			if (iterator.Count == 0)
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

			return XmlConvert.ToString(sum); //XmlConvert.ToString(sum);
		}

	    public string sum(XPathNodeIterator iterator) => Sum(iterator);

        /// <summary>
        /// Implements the following function 
        ///    string date:duration()
        /// </summary>
        /// <returns>seconds since the beginning of the epoch until now</returns>        
        public string Duration()
		{
			return Duration(Seconds());
		}

	    public string duration() => Duration();

        /// <summary>
        /// Implements the following function 
        ///    string date:duration(number)
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>        
        public string Duration(double seconds)
		{
			return XmlConvert.ToString(TimeSpan.FromSeconds(seconds));
		}

	    public string duration(double seconds) => Duration(seconds);
	}
}
