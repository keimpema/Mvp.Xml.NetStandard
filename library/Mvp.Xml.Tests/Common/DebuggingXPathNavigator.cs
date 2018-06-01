using System;
using System.Xml;
using System.Xml.XPath;

namespace Mvp.Xml.Tests
{
	/// <summary>
	/// Allows tracking of what members of the 
	/// <see cref="XPathNavigator"/> class are being called.
	/// </summary>
	public sealed class DebuggingXPathNavigator : XPathNavigator
	{
		XPathNavigator _navigator; 
		public DebuggingXPathNavigator(XPathNavigator navigator)
		{
			_navigator = navigator;
		}

		#region XPathNavigator overrides

		#region Properties
		
		/// <summary>
		/// See <see cref="XPathNavigator.BaseURI"/>.
		/// </summary>
		public override String BaseURI 
		{
			get
			{ 
				System.Diagnostics.Debug.WriteLine("BaseURI = " + _navigator.BaseURI);
				return _navigator.BaseURI;
			} 
		}

		/// <summary>
		/// See <see cref="XPathNavigator.HasAttributes"/>.
		/// </summary>
		public override bool HasAttributes 
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("HasAttributes = " + _navigator.HasAttributes);
				return _navigator.HasAttributes;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.HasChildren"/>.
		/// </summary>
		public override bool HasChildren 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("HasAttributes = " + _navigator.HasChildren);
				return _navigator.HasChildren;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.IsEmptyElement"/>.
		/// </summary>
		public override bool IsEmptyElement 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("IsEmptyElement = " + _navigator.IsEmptyElement);
				return _navigator.IsEmptyElement;
			}
		}
      
		/// <summary>
		/// See <see cref="XPathNavigator.LocalName"/>.
		/// </summary>
		public override string LocalName 
		{         
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("LocalName = " + _navigator.LocalName);
				return _navigator.LocalName;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.Name"/>.
		/// </summary>
		public override string Name 
		{ 
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("Name = " + _navigator.Name);
				return _navigator.Name;
			}
		}
		
		/// <summary>
		/// See <see cref="XPathNavigator.NamespaceURI"/>.
		/// </summary>
		public override string NamespaceURI 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("NamespaceURI = " + _navigator.NamespaceURI);
				return _navigator.NamespaceURI;
			}
		}
      
		/// <summary>
		/// See <see cref="XPathNavigator.NameTable"/>.
		/// </summary>
		public override XmlNameTable NameTable 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("NameTable = " + _navigator.NameTable);
				return _navigator.NameTable;
			}
		}		

		/// <summary>
		/// See <see cref="XPathNavigator.NodeType"/>.
		/// </summary>
		public override XPathNodeType NodeType 
		{ 
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("NodeType = " + _navigator.NodeType);
				return _navigator.NodeType;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.Prefix"/>.
		/// </summary>
		public override string Prefix
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("Prefix = " + _navigator.Prefix);
				return _navigator.Prefix;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.Value"/>.
		/// </summary>
		public override string Value 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("Value = " + _navigator.Value);
				return _navigator.Value;
			}
		}

		/// <summary>
		/// See <see cref="XPathNavigator.XmlLang"/>.
		/// </summary>
		public override string XmlLang 
		{
			get 
			{ 
				System.Diagnostics.Debug.WriteLine("XmlLang = " + _navigator.XmlLang);
				return _navigator.XmlLang;
			}
		}
      
		#endregion Properties

		#region Methods

		/// <summary>
		/// Creates new cloned version of the <see cref="SubtreeeXPathNavigator"/>.
		/// </summary>
		/// <returns>Cloned copy of the <see cref="SubtreeeXPathNavigator"/>.</returns>
		public override XPathNavigator Clone() 
		{
			System.Diagnostics.Debug.WriteLine("Clone()");
			return new DebuggingXPathNavigator(_navigator.Clone());
		}

		/// <summary>
		/// See <see cref="XPathNavigator.IsSamePosition"/>.
		/// </summary>
		public override bool IsSamePosition(XPathNavigator other) 
		{
			bool res = _navigator.IsSamePosition(other);
			System.Diagnostics.Debug.WriteLine("IsSamePosition(" + (other != null ? other.ToString() : "null") + ") = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToId"/>.
		/// </summary>
		public override bool MoveToId(string id) 
		{
			bool res = _navigator.MoveToId(id);
			System.Diagnostics.Debug.WriteLine("MoveToId(" + id + ") = " + res);
			return res;
		}

		#region Element methods

		/// <summary>
		/// See <see cref="XPathNavigator.MoveTo"/>.
		/// </summary>
		public override bool MoveTo(XPathNavigator other) 
		{
			bool res = _navigator.MoveTo(other);
			System.Diagnostics.Debug.WriteLine("MoveTo(" + (other != null ? other.ToString() : "null") + ") = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirst"/>.
		/// </summary>
		public override bool MoveToFirst() 
		{		    
			bool res = _navigator.MoveToFirst();
			System.Diagnostics.Debug.WriteLine("MoveToFirst() = " + res);
			return res;
		}
		
		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstChild"/>.
		/// </summary>
		public override bool MoveToFirstChild() 
		{      
			bool res = _navigator.MoveToFirstChild();
			System.Diagnostics.Debug.WriteLine("MoveToFirstChild() = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNext"/>.
		/// </summary>
		public override bool MoveToNext() 
		{
			bool res = _navigator.MoveToNext();
			System.Diagnostics.Debug.WriteLine("MoveToNext() = " + res);
			return res;
		}
        
		/// <summary>
		/// See <see cref="XPathNavigator.MoveToParent"/>.
		/// </summary>
		public override bool MoveToParent() 
		{
			bool res = _navigator.MoveToParent();
			System.Diagnostics.Debug.WriteLine("MoveToParent() = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToPrevious"/>.
		/// </summary>
		public override bool MoveToPrevious() 
		{
			bool res = _navigator.MoveToPrevious();
			System.Diagnostics.Debug.WriteLine("MoveToPrevious() = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToRoot"/>.
		/// </summary>
		public override void MoveToRoot() 
		{
			System.Diagnostics.Debug.WriteLine("MoveToRoot()");
			_navigator.MoveToRoot();
		}

		#endregion Element methods

		#region Attribute methods

		/// <summary>
		/// See <see cref="XPathNavigator.GetAttribute"/>.
		/// </summary>
		public override string GetAttribute(string localName, string namespaceURI) 
		{
			string attr = _navigator.GetAttribute(localName, namespaceURI);
			System.Diagnostics.Debug.WriteLine("GetAttribute(" + localName + ", " + namespaceURI + ") = " + attr);
			return attr;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToAttribute"/>.
		/// </summary>
		public override bool MoveToAttribute(string localName, string namespaceURI) 
		{
			bool res = _navigator.MoveToAttribute(localName, namespaceURI);
			System.Diagnostics.Debug.WriteLine("MoveToAttribute(" + localName + ", " + namespaceURI + ") = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstAttribute"/>.
		/// </summary>
		public override bool MoveToFirstAttribute() 
		{
			bool res = _navigator.MoveToFirstAttribute();
			System.Diagnostics.Debug.WriteLine("MoveToFirstAttribute() = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNextAttribute"/>.
		/// </summary>
		public override bool MoveToNextAttribute() 
		{
			bool res = _navigator.MoveToNextAttribute();
			System.Diagnostics.Debug.WriteLine("MoveToNextAttribute() = " + res);
			return res;
		}
		
		#endregion Attribute methods

		#region Namespace methods

		/// <summary>
		/// See <see cref="XPathNavigator.GetNamespace"/>.
		/// </summary>
		public override string GetNamespace(string localName) 
		{
			string attr = _navigator.GetNamespace(localName);
			System.Diagnostics.Debug.WriteLine("GetNamespace(" + localName + ") = " + attr);
			return attr;
		}
		
		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNamespace"/>.
		/// </summary>
		public override bool MoveToNamespace(string @namespace) 
		{
			bool res = _navigator.MoveToNamespace(@namespace);
			System.Diagnostics.Debug.WriteLine("MoveToNamespace(" + @namespace + ") = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToFirstNamespace"/>.
		/// </summary>
		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) 
		{
			bool res = _navigator.MoveToFirstNamespace();
			System.Diagnostics.Debug.WriteLine("MoveToFirstNamespace() = " + res);
			return res;
		}

		/// <summary>
		/// See <see cref="XPathNavigator.MoveToNextNamespace"/>.
		/// </summary>
		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) 
		{
			bool res = _navigator.MoveToNextNamespace(namespaceScope);
			System.Diagnostics.Debug.WriteLine("MoveToNextNamespace(" + namespaceScope + ") = " + res);
			return res;
		}
		
		#endregion Namespace methods

		#endregion Methods

		#endregion
	}
}
