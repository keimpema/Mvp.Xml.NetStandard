using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// An <see cref="XPathNodeIterator"/> that allows 
	/// arbitrary addition of the <see cref="XPathNavigator"/> 
	/// nodes that belong to the set.
	/// </summary>
	/// <remarks>
	/// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
	/// <para>Contributors: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
	/// </remarks>
	public class XPathNavigatorIterator : XPathNodeIterator
	{
	    private readonly List<XPathNavigator> navigators;
		private int position = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>.
		/// </summary>
		public XPathNavigatorIterator()
		{
			navigators = new List<XPathNavigator>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>
		/// with given initial capacity.
		/// </summary>
		public XPathNavigatorIterator(int capacity)
		{
			navigators = new List<XPathNavigator>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>, 
		/// using the received navigator as the initial item in the set. 
		/// </summary>
		public XPathNavigatorIterator(XPathNavigator navigator)
			: this()
		{
			navigators.Add(navigator);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>
		/// using given list of navigators.
		/// </summary>
		public XPathNavigatorIterator(XPathNodeIterator iterator)
			: this(iterator, false) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>
		/// using given list of navigators.
		/// </summary>
		public XPathNavigatorIterator(XPathNodeIterator iterator, bool removeDuplicates)
			: this()
		{
			XPathNodeIterator it = iterator.Clone();

			while (it.MoveNext())
			{
				if (removeDuplicates)
				{
					if (Contains(it.Current))
					{
						continue;
					}
				}

				Add(it.Current.Clone());
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XPathNavigatorIterator"/>
		/// using given list of navigators.
		/// </summary>
		public XPathNavigatorIterator(List<XPathNavigator> navigators) => this.navigators = navigators;

	    /// <summary>
		/// Adds a <see cref="XPathNavigator"/> to the set.
		/// </summary>
		/// <param name="navigator">The navigator to add. It's cloned automatically.</param>
		public void Add(XPathNavigator navigator)
		{
			if (position != -1)
			{
			    throw new InvalidOperationException(Properties.Resources.XPathNavigatorIterator_CantAddAfterMove);
			}

		    navigators.Add(navigator.Clone());
		}

		/// <summary>
		/// Adds a <see cref="XPathNodeIterator"/> containing a set of navigators to add.
		/// </summary>
		/// <param name="iterator">The set of navigators to add. Each one is cloned automatically.</param>
		public void Add(XPathNodeIterator iterator)
		{
			if (position != -1)
			{
			    throw new InvalidOperationException(
			        Properties.Resources.XPathNavigatorIterator_CantAddAfterMove);
			}

		    while (iterator.MoveNext())
			{
				navigators.Add(iterator.Current.Clone());
			}
		}

		/// <summary>
		/// Adds a <see cref="IEnumerable&lt;XPathNavigator&gt;"/> containing a set of navigators to add.
		/// </summary>
		/// <param name="navigatorsToAdd">The set of navigators to add. Each one is cloned automatically.</param>
		public void Add(IEnumerable<XPathNavigator> navigatorsToAdd)
		{
			if (position != -1)
			{
			    throw new InvalidOperationException(
			        Properties.Resources.XPathNavigatorIterator_CantAddAfterMove);
			}

		    foreach (XPathNavigator navigator in navigatorsToAdd)
			{
				navigators.Add(navigator.Clone());
			}
		}

		/// <summary>
		/// Determines whether the list contains a navigator positioned at the same 
		/// location as the specified XPathNavigator. This 
		/// method relies on the IsSamePositon() method of the XPathNavightor. 
		/// </summary>
		/// <param name="value">The object to locate in the list.</param>
		/// <returns>true if the object is found in the list; otherwise, false.</returns>
		public bool Contains(XPathNavigator value)
		{
			foreach (XPathNavigator nav in navigators)
			{
				if (nav.IsSamePosition(value))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether the list contains a navigator whose Value property matches
		/// the target value
		/// </summary>
		/// <param name="value">The value to locate in the list.</param>
		/// <returns>true if the value is found in the list; otherwise, false.</returns>
		public bool ContainsValue(string value)
		{
			foreach (XPathNavigator nav in navigators)
			{
				if (nav.Value.Equals(value))
				{
					return true;
				}
			}
			return false;
		}


	    /// <summary>
	    /// Gets or sets the element at the specified index
	    /// </summary>
	    public XPathNavigator this[int index] => navigators[index];

		/// <summary>
		/// Removes the list item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public void RemoveAt(int index) => navigators.RemoveAt(index);


	    /// <summary>
		/// Resets the iterator.
		/// </summary>
		public void Reset() => position = -1;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.Clone"/>.
		/// </summary>
		public override XPathNodeIterator Clone()
		{
			return new XPathNavigatorIterator(new List<XPathNavigator>(navigators));
		}

		/// <summary>
		/// See <see cref="XPathNodeIterator.Count"/>.
		/// </summary>
		public override int Count => navigators.Count;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.Current"/>.
		/// </summary>
		public override XPathNavigator Current => position == -1 ? null : navigators[position];

	    /// <summary>
		/// See <see cref="XPathNodeIterator.CurrentPosition"/>.
		/// </summary>
		public override int CurrentPosition => position + 1;

	    /// <summary>
		/// See <see cref="XPathNodeIterator.MoveNext"/>.
		/// </summary>
		public override bool MoveNext()
		{
			if (navigators.Count == 0)
			{
			    return false;
			}

		    position++;
			return position < navigators.Count;
		}
	}
}
