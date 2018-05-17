using System.Collections.Generic;

namespace Mvp.Xml.Common.Serialization
{
	/// <summary>
	/// Helper class to simpify sorting
	/// strings (Not really necessary in Whidbey).
	/// </summary>
	public class StringSorter
	{
	    private readonly List<string> list = new List<string>();

	    /// <summary>
		/// Add a string to sort
		/// </summary>
		/// <param name="s"></param>
		public void AddString(string s)
		{
			list.Add(s);
		}

		/// <summary>
		/// Sort the strings that were added by calling
		/// <see cref="AddString"/>
		/// </summary>
		/// <returns>A sorted string array.</returns>
		public string[] GetOrderedArray()
		{
			list.Sort();
			return list.ToArray();
		}
	}
}
