using System;
using System.Globalization;

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Helper class with guard checks.
	/// </summary>
	internal static class Guard
	{
		/// <summary>
		/// Checks an argument to ensure it isn't null.
		/// </summary>
		/// <param name="value">The argument value to check.</param>
		/// <param name="argumentName">The name of the argument.</param>
		public static void ArgumentNotNull(object value, string argumentName)
		{
			if (value == null)
			{
			    throw new ArgumentNullException(argumentName);
			}
		}

		/// <summary>
		/// Checks a string argument to ensure it isn't null or empty.
		/// </summary>
		/// <param name="value">The argument value to check.</param>
		/// <param name="argumentName">The name of the argument.</param>
		public static void ArgumentNotNullOrEmptyString(string value, string argumentName)
		{
			ArgumentNotNull(value, argumentName);

			if (value.Length == 0)
			{
			    throw new ArgumentException(
			        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Arg_NullOrEmpty), argumentName);
			}
		}
	}
}
