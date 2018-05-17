using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// <see cref="XPathNodeIterator"/> over a single node. Can be used to return a single
	/// node out of an XSLT or XPath extension function.
	/// </summary>
	/// <remarks>
	/// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
	/// </remarks>
	public class SingletonXPathNodeIterator : XPathNodeIterator
	{
		private readonly XPathNavigator navigator;
		private int position;

	    /// <summary>
		/// Creates new instance of SingletonXPathNodeIterator over
		/// given node.
		/// </summary>
		public SingletonXPathNodeIterator(XPathNavigator nav)
		{
			navigator = nav;
		}

	    /// <summary>
		/// See <see cref="XPathNodeIterator.Clone()"/>
		/// </summary>
		public override XPathNodeIterator Clone()
		{
			return new SingletonXPathNodeIterator(navigator.Clone());
		}

		/// <summary>
		/// Always 1. See <see cref="XPathNodeIterator.Count"/>
		/// </summary>
		public override int Count { get; } = 1;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.Current"/>
		/// </summary>
		public override XPathNavigator Current => navigator;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.CurrentPosition"/>
		/// </summary>
		public override int CurrentPosition => position;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.MoveNext()"/>
		/// </summary>
		public override bool MoveNext()
		{
			if (position == 0)
			{
				position = 1;
				return true;
			}
			return false;
		}
	}
}
