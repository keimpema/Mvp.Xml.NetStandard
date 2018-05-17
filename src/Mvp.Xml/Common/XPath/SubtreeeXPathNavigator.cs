using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath
{
	/// <summary>	
	/// Allows to navigate a subtree of an <see cref="IXPathNavigable"/> source, 
	/// by limiting the scope of the navigator to that received 
	/// at construction time.
	/// </summary>
	/// <remarks>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a>
	/// <para>See http://weblogs.asp.net/cazzu/archive/2004/06/24/164243.aspx</para>
	/// </remarks>
	public class SubtreeXPathNavigator : XPathNavigator
	{
	    // The current location.
	    private XPathNavigator navigator;
		// The node that limits the scope.
	    private readonly XPathNavigator root;
		// Whether we're at the root node (parent of the first child).
	    // Whether XML fragment navigation is enabled.
	    private readonly bool fragment;

		/// <summary>
		/// Creates SubtreeXPathNavigator over specified XPathNavigator.
		/// </summary>
		/// <param name="navigator">Navigator that determines scope.</param>
		/// <remarks>The incoming navigator is cloned upon construction, 
		/// which isolates the calling code from movements to the 
		/// <see cref="SubtreeXPathNavigator"/>.</remarks>
		public SubtreeXPathNavigator(XPathNavigator navigator)
			: this(navigator, false)
		{
		}

		/// <summary>
		/// Creates SubtreeXPathNavigator over specified XPathNavigator.
		/// </summary>
		/// <param name="navigator">Navigator that determines scope.</param>
		/// <param name="enableFragment">Whether the navigator should be able to 
		/// move among all siblings of the <paramref name="navigator"/> defining the 
		/// scope.</param>
		/// <remarks>The incoming navigator is cloned upon construction, 
		/// which isolates the calling code from movements to the 
		/// <see cref="SubtreeXPathNavigator"/>.</remarks>
		public SubtreeXPathNavigator(XPathNavigator navigator, bool enableFragment)
		{
			this.navigator = navigator.Clone();
			root = navigator.Clone();
			fragment = enableFragment;
		}

		private SubtreeXPathNavigator(XPathNavigator root, XPathNavigator current,
			bool atRoot, bool enableFragment)
		{
			this.root = root.Clone();
			navigator = current.Clone();
			AtRoot = atRoot;
			fragment = enableFragment;
		}

	    /// <summary>
		/// Determines whether the navigator is on the root node (before the first child).
		/// </summary>
		private bool AtRoot { get; set; } = true;

	    /// <summary>
		/// Determines whether the navigator is at the same position as the "document element".
		/// </summary>
		private bool IsTop => navigator.IsSamePosition(root);

	    /// <summary>
		/// See <see cref="XPathNavigator.BaseURI"/>.
		/// </summary>
		public override string BaseURI => AtRoot ? string.Empty : navigator.BaseURI;

	    /// <summary>
		/// See <see cref="XPathNavigator.HasAttributes"/>.
		/// </summary>
		public override bool HasAttributes => !AtRoot && navigator.HasAttributes;

	    /// <summary>
		/// See <see cref="XPathNavigator.HasChildren"/>.
		/// </summary>
		public override bool HasChildren => AtRoot || navigator.HasChildren;

	    /// <summary>
		/// See <see cref="XPathNavigator.IsEmptyElement"/>.
		/// </summary>
		public override bool IsEmptyElement => !AtRoot && navigator.IsEmptyElement;

	    /// <summary>
		/// See <see cref="XPathNavigator.LocalName"/>.
		/// </summary>
		public override string LocalName => AtRoot ? string.Empty : navigator.LocalName;

	    /// <summary>
		/// See <see cref="XPathNavigator.Name"/>.
		/// </summary>
		public override string Name => AtRoot ? string.Empty : navigator.Name;

	    /// <summary>
		/// See <see cref="XPathNavigator.NamespaceURI"/>.
		/// </summary>
		public override string NamespaceURI => AtRoot ? string.Empty : navigator.NamespaceURI;

	    /// <summary>
		/// See <see cref="XPathNavigator.NameTable"/>.
		/// </summary>
		public override XmlNameTable NameTable => navigator.NameTable;

	    /// <summary>
		/// See <see cref="XPathNavigator.NodeType"/>.
		/// </summary>
		public override XPathNodeType NodeType => AtRoot ? XPathNodeType.Root : navigator.NodeType;

	    /// <summary>
		/// See <see cref="XPathNavigator.Prefix"/>.
		/// </summary>
		public override string Prefix => AtRoot ? string.Empty : navigator.Prefix;

	    /// <summary>
		/// See <see cref="XPathItem.Value"/>.
		/// </summary>
		public override string Value => AtRoot ? string.Empty : navigator.Value;

	    /// <summary>
		/// See <see cref="XPathNavigator.XmlLang"/>.
		/// </summary>
		public override string XmlLang => AtRoot ? string.Empty : navigator.XmlLang;

	    /// <summary>
		/// Creates new cloned version of the <see cref="SubtreeXPathNavigator"/>.
		/// </summary>
		/// <returns>Cloned copy of the <see cref="SubtreeXPathNavigator"/>.</returns>
		public override XPathNavigator Clone()
		{
			return new SubtreeXPathNavigator(root, navigator, AtRoot, fragment);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.IsSamePosition"/>.
		/// </summary>
		public override bool IsSamePosition(XPathNavigator other)
		{
			if (!(other is SubtreeXPathNavigator))
			{
			    return false;
			}

		    var nav = (SubtreeXPathNavigator)other;
			return nav.AtRoot == AtRoot &&
				nav.navigator.IsSamePosition(navigator) &&
				nav.root.IsSamePosition(root);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToId"/>.
		/// </summary>
		public override bool MoveToId(string id)
		{
			return navigator.MoveToId(id);
		}

	    /// <summary>
		/// See <see cref="XPathNavigator.MoveTo"/>.
		/// </summary>
		public override bool MoveTo(XPathNavigator other)
		{
			if (!(other is SubtreeXPathNavigator))
			{
			    return false;
			}

		    return navigator.MoveTo(((SubtreeXPathNavigator)other).navigator);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirst"/>.
		/// </summary>
		public override bool MoveToFirst()
		{
			if (AtRoot)
			{
			    return false;
			}

		    if (IsTop)
		    {
		        if (!fragment)
				{
					return false;
				}

		        if (root.MoveToFirst())
		        {
		            navigator.MoveToFirst();
		            return true;
		        }
		    }

			return navigator.MoveToNext();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstChild"/>.
		/// </summary>
		public override bool MoveToFirstChild()
		{
			if (AtRoot)
			{
				AtRoot = false;
				return true;
			}

			return navigator.MoveToFirstChild();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNext()"/>.
		/// </summary>
		public override bool MoveToNext()
		{
			if (AtRoot)
			{
			    return false;
			}

		    if (IsTop)
		    {
		        if (!fragment)
				{
					return false;
				}

		        if (root.MoveToNext())
		        {
		            navigator.MoveToNext();
		            return true;
		        }
		    }

			return navigator.MoveToNext();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToParent"/>.
		/// </summary>
		public override bool MoveToParent()
		{
			if (AtRoot)
			{
			    return false;
			}

		    if (IsTop)
			{
				AtRoot = true;
				return true;
			}

			return navigator.MoveToParent();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToPrevious"/>.
		/// </summary>
		public override bool MoveToPrevious()
		{
			if (AtRoot)
			{
			    return false;
			}

		    if (IsTop)
		    {
		        if (!fragment)
				{
					return false;
				}

		        if (root.MoveToPrevious())
		        {
		            navigator.MoveToPrevious();
		            return true;
		        }
		    }

			return navigator.MoveToPrevious();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToRoot"/>.
		/// </summary>
		public override void MoveToRoot()
		{
			navigator = root.Clone();
			AtRoot = true;
		}

	    /// <summary>
		/// See <see cref="XPathNavigator.GetAttribute"/>.
		/// </summary>
		public override string GetAttribute(string localName, string namespaceUri)
		{
			return AtRoot ? string.Empty : navigator.GetAttribute(localName, namespaceUri);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToAttribute"/>.
		/// </summary>
		public override bool MoveToAttribute(string localName, string namespaceUri)
		{
			return !AtRoot && navigator.MoveToAttribute(localName, namespaceUri);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstAttribute"/>.
		/// </summary>
		public override bool MoveToFirstAttribute()
		{
			return !AtRoot && navigator.MoveToFirstAttribute();
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNextAttribute"/>.
		/// </summary>
		public override bool MoveToNextAttribute()
		{
			return !AtRoot && navigator.MoveToNextAttribute();
		}

	    /// <summary>
		/// See <see cref="XPathNavigator.GetNamespace"/>.
		/// </summary>
		public override string GetNamespace(string localName)
		{
			return AtRoot ? string.Empty : navigator.GetNamespace(localName);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNamespace"/>.
		/// </summary>
		public override bool MoveToNamespace(string @namespace)
		{
			return !AtRoot && navigator.MoveToNamespace(@namespace);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstNamespace(XPathNamespaceScope)"/>.
		/// </summary>
		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
		{
			return !AtRoot && navigator.MoveToFirstNamespace(namespaceScope);
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNextNamespace(XPathNamespaceScope)"/>.
		/// </summary>
		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
		{
			return !AtRoot && navigator.MoveToNextNamespace(namespaceScope);
		}
	}
}