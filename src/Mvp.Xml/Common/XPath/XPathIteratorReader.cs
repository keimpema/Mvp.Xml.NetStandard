using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Mvp.Xml.Common.XPath
{
	/// <summary>
	/// Provides an <see cref="XmlReader"/> over an 
	/// <see cref="XPathNodeIterator"/>.
	/// </summary>
	/// <remarks>
	/// The reader exposes a new root element enclosing all navigators from the 
	/// iterator. This root node is configured in the constructor, by 
	/// passing the desired name and optional namespace for it.
	/// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
	/// See: http://weblogs.asp.net/cazzu/archive/2004/04/26/120684.aspx
	/// </remarks>
	public class XPathIteratorReader : XmlTextReader, IXmlSerializable
	{
	    // Holds the current child being read.
		XmlReader current;

		// Holds the iterator passed to the ctor. 
		XPathNodeIterator iterator;

		// The name for the root element.
		XmlQualifiedName rootname;

	    /// <summary>
		/// Parameterless constructor for XML serialization.
		/// </summary>
		/// <remarks>Supports the .NET serialization infrastructure. Don't use this 
		/// constructor in your regular application.</remarks>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public XPathIteratorReader()
		{
		}

		/// <summary>
		/// Initializes the reader, using the default &lt;root&gt; element.
		/// </summary>
		/// <param name="iterator">The iterator to expose as a single reader.</param>
		public XPathIteratorReader(XPathNodeIterator iterator)
			: this(iterator, "root", string.Empty)
		{
		}

		/// <summary>
		/// Initializes the reader.
		/// </summary>
		/// <param name="iterator">The iterator to expose as a single reader.</param>
		/// <param name="rootName">The name to use for the enclosing root element.</param>
		public XPathIteratorReader(XPathNodeIterator iterator, string rootName)
			: this(iterator, rootName, string.Empty)
		{
		}

		/// <summary>
		/// Initializes the reader.
		/// </summary>
		/// <param name="iterator">The iterator to expose as a single reader.</param>
		/// <param name="rootName">The name to use for the enclosing root element.</param>
		/// <param name="ns">The namespace URI of the root element.</param>
		public XPathIteratorReader(XPathNodeIterator iterator, string rootName, string ns)
			: base(new StringReader(string.Empty))
		{
			this.iterator = iterator.Clone();
			current = new FakedRootReader(rootName, ns, XmlNodeType.Element);
			rootname = new XmlQualifiedName(rootName, ns);
		}

	    /// <summary>
		/// Returns the XML representation of the current node and all its children.
		/// </summary>
		private string Serialize()
		{
			StringWriter sw = new StringWriter(System.Globalization.CultureInfo.CurrentCulture);
			XmlTextWriter tw = new XmlTextWriter(sw);
			tw.WriteNode(this, false);

			sw.Flush();
			return sw.ToString();
		}

	    /// <summary>See <see cref="XmlReader.AttributeCount"/></summary>
		public override int AttributeCount => current.AttributeCount;

	    /// <summary>See <see cref="XmlReader.BaseURI"/></summary>
		public override string BaseURI => current.BaseURI;

	    /// <summary>See <see cref="XmlReader.Depth"/></summary>
		public override int Depth => current.Depth + 1;

	    /// <summary>See <see cref="XmlReader.EOF"/></summary>
		public override bool EOF => current.ReadState == ReadState.EndOfFile || current.ReadState == ReadState.Closed;

	    /// <summary>See <see cref="XmlReader.HasValue"/></summary>
		public override bool HasValue => current.HasValue;

	    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
		public override bool IsDefault => false;

	    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
		public override bool IsEmptyElement => current.IsEmptyElement;

	    /// <summary>See <see cref="XmlReader.this[string, string]"/></summary>
		public override string this[string name, string ns] => current[name, ns];

	    /// <summary>See <see cref="XmlReader.this[string]"/></summary>
		public override string this[string name] => current[name, string.Empty];

	    /// <summary>See <see cref="XmlReader.this[int]"/></summary>
		public override string this[int i] => current[i];

	    /// <summary>See <see cref="XmlReader.LocalName"/></summary>
		public override string LocalName => current.LocalName;

	    /// <summary>See <see cref="XmlReader.Name"/></summary>
		public override string Name => current.Name;

	    /// <summary>See <see cref="XmlReader.NamespaceURI"/></summary>
		public override string NamespaceURI => current.NamespaceURI;

	    /// <summary>See <see cref="XmlReader.NameTable"/></summary>
		public override XmlNameTable NameTable => current.NameTable;

	    /// <summary>See <see cref="XmlReader.NodeType"/></summary>
		public override XmlNodeType NodeType => current.NodeType;

	    /// <summary>See <see cref="XmlReader.Prefix"/></summary>
		public override string Prefix => current.Prefix;

	    /// <summary>See <see cref="XmlReader.QuoteChar"/></summary>
		public override char QuoteChar => current.QuoteChar;

	    /// <summary>See <see cref="XmlReader.ReadState"/></summary>
		public override ReadState ReadState => current.ReadState;

	    /// <summary>See <see cref="XmlReader.Value"/></summary>
		public override string Value => current.Value;

	    /// <summary>See <see cref="XmlReader.XmlLang"/></summary>
		public override string XmlLang => current.XmlLang;

	    /// <summary>See <see cref="XmlReader.XmlSpace"/></summary>
		public override XmlSpace XmlSpace => XmlSpace.Default;

	    /// <summary>See <see cref="XmlReader.Close"/></summary>
		public override void Close()
		{
			current.Close();
		}

		/// <summary>See <see cref="XmlReader.GetAttribute(string, string)"/></summary>
		public override string GetAttribute(string name, string ns)
		{
			return current.GetAttribute(name, ns);
		}

		/// <summary>See <see cref="XmlReader.GetAttribute(string)"/></summary>
		public override string GetAttribute(string name)
		{
			return current.GetAttribute(name);
		}

		/// <summary>See <see cref="XmlReader.GetAttribute(int)"/></summary>
		public override string GetAttribute(int i)
		{
			return current.GetAttribute(i);
		}

		/// <summary>See <see cref="XmlReader.LookupNamespace"/></summary>
		public override string LookupNamespace(string prefix)
		{
			return current.LookupNamespace(prefix);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(string, string)"/></summary>
		public override bool MoveToAttribute(string name, string ns)
		{
			return current.MoveToAttribute(name, ns);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(string)"/></summary>
		public override bool MoveToAttribute(string name)
		{
			return current.MoveToAttribute(name);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(int)"/></summary>
		public override void MoveToAttribute(int i)
		{
			current.MoveToAttribute(i);
		}

	    /// <summary>See <see cref="XmlReader.MoveToElement"/></summary>
		public override bool MoveToElement()
		{
			return current.MoveToElement();
		}

		/// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/></summary>
		public override bool MoveToFirstAttribute()
		{
			return current.MoveToFirstAttribute();
		}

		/// <summary>See <see cref="XmlReader.MoveToNextAttribute"/></summary>
		public override bool MoveToNextAttribute()
		{
			return current.MoveToNextAttribute();
		}

		/// <summary>See <see cref="XmlReader.Read"/></summary>
		public override bool Read()
		{
			// Return fast if state is no appropriate.
			if (current.ReadState == ReadState.Closed || current.ReadState == ReadState.EndOfFile)
			{
			    return false;
			}

		    if (current.Read())
		    {
		        return true;
		    }

		    if (iterator.MoveNext())
		    {
		        // Just move to the next node and create the reader.
		        current = new SubtreeXPathNavigator(iterator.Current).ReadSubtree();
		        return current.Read();
		    }

		    if (current is FakedRootReader && current.NodeType == XmlNodeType.EndElement)
		    {
		        // We're done!
		        return false;
		    }

		    // We read all nodes in the iterator. Return to faked root end element.
		    current = new FakedRootReader(rootname.Name, rootname.Namespace, XmlNodeType.EndElement);
		    return true;
		}

		/// <summary>See <see cref="XmlReader.ReadAttributeValue"/></summary>
		public override bool ReadAttributeValue()
		{
			return current.ReadAttributeValue();
		}

		/// <summary>See <see cref="XmlReader.ReadInnerXml"/></summary>
		public override string ReadInnerXml()
		{
		    return Read() ? Serialize() : string.Empty;
		}

		/// <summary>See <see cref="XmlReader.ReadOuterXml"/></summary>
		public override string ReadOuterXml()
		{
			if (current.ReadState != ReadState.Interactive)
			{
			    return string.Empty;
			}

		    return Serialize();
		}

		/// <summary>See <see cref="XmlReader.Read"/></summary>
		public override void ResolveEntity()
		{
			// Not supported.
		}

	    /// <summary>
		/// See <see cref="IXmlSerializable.WriteXml"/>.
		/// </summary>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteNode(this, false);
		}

		/// <summary>
		/// See <see cref="IXmlSerializable.GetSchema"/>.
		/// </summary>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// See <see cref="IXmlSerializable.ReadXml"/>.
		/// </summary>
		public void ReadXml(XmlReader reader)
		{
			XPathDocument doc = new XPathDocument(reader);
			XPathNavigator nav = doc.CreateNavigator();

			// Pull the faked root out.
			nav.MoveToFirstChild();
			rootname = new XmlQualifiedName(nav.LocalName, nav.NamespaceURI);

			// Get iterator for all child nodes.
			iterator = nav.SelectChildren(XPathNodeType.All);
		}

	    private class FakedRootReader : XmlReader
		{
			public FakedRootReader(string name, string ns,
				XmlNodeType nodeType)
			{
				rootName = name;
				NamespaceURI = ns;
				nodetype = nodeType;
				state = nodeType == XmlNodeType.Element ?
					ReadState.Initial : ReadState.Interactive;
			}

		    /// <summary>See <see cref="XmlReader.AttributeCount"/></summary>
			public override int AttributeCount { get; } = 0;

		    /// <summary>See <see cref="XmlReader.BaseURI"/></summary>
			public override string BaseURI { get; } = string.Empty;

		    /// <summary>See <see cref="XmlReader.Depth"/></summary>
			public override int Depth { get; } = -1;

		    /// <summary>See <see cref="XmlReader.EOF"/></summary>
			public override bool EOF => state == ReadState.EndOfFile;

		    /// <summary>See <see cref="XmlReader.HasValue"/></summary>
			public override bool HasValue { get; } = false;

		    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
			public override bool IsDefault { get; } = false;

		    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
			public override bool IsEmptyElement { get; } = false;

		    /// <summary>See <see cref="XmlReader.this[string, string]"/></summary>
			public override string this[string name, string ns] => null;

		    /// <summary>See <see cref="XmlReader.this[string]"/></summary>
			public override string this[string name] => null;

		    /// <summary>See <see cref="XmlReader.this[int]"/></summary>
			public override string this[int i] => null;

		    /// <summary>See <see cref="XmlReader.LocalName"/></summary>
			public override string LocalName => rootName;

		    private readonly string rootName;

			/// <summary>See <see cref="XmlReader.Name"/></summary>
			public override string Name => rootName;

		    /// <summary>See <see cref="XmlReader.NamespaceURI"/></summary>
			public override string NamespaceURI { get; }

		    /// <summary>See <see cref="XmlReader.NameTable"/></summary>
			public override XmlNameTable NameTable { get; } = null;

		    /// <summary>See <see cref="XmlReader.NodeType"/></summary>
			public override XmlNodeType NodeType => state == ReadState.Initial ? XmlNodeType.None : nodetype;

		    private readonly XmlNodeType nodetype;

			/// <summary>See <see cref="XmlReader.Prefix"/></summary>
			public override string Prefix { get; } = string.Empty;

		    /// <summary>See <see cref="XmlReader.QuoteChar"/></summary>
			public override char QuoteChar { get; } = '"';

		    /// <summary>See <see cref="XmlReader.ReadState"/></summary>
			public override ReadState ReadState => state;

		    private ReadState state;

			/// <summary>See <see cref="XmlReader.Value"/></summary>
			public override string Value { get; } = string.Empty;

		    /// <summary>See <see cref="XmlReader.XmlLang"/></summary>
			public override string XmlLang { get; } = string.Empty;

		    /// <summary>See <see cref="XmlReader.XmlSpace"/></summary>
			public override XmlSpace XmlSpace { get; } = XmlSpace.Default;

		    /// <summary>See <see cref="XmlReader.Close"/></summary>
			public override void Close() => state = ReadState.Closed;

		    /// <summary>See <see cref="XmlReader.GetAttribute(string, string)"/></summary>
			public override string GetAttribute(string name, string ns) => null;

		    /// <summary>See <see cref="XmlReader.GetAttribute(string)"/></summary>
			public override string GetAttribute(string name) => null;

		    /// <summary>See <see cref="XmlReader.GetAttribute(int)"/></summary>
			public override string GetAttribute(int i) => null;

		    /// <summary>See <see cref="XmlReader.LookupNamespace"/></summary>
			public override string LookupNamespace(string prefix) => null;

		    /// <summary>See <see cref="XmlReader.MoveToAttribute(string, string)"/></summary>
			public override bool MoveToAttribute(string name, string ns) => false;

		    /// <summary>See <see cref="XmlReader.MoveToAttribute(string)"/></summary>
			public override bool MoveToAttribute(string name) => false;

		    /// <summary>See <see cref="XmlReader.MoveToAttribute(int)"/></summary>
			public override void MoveToAttribute(int i)
			{
			}

			public override XmlNodeType MoveToContent()
			{
				if (state == ReadState.Initial)
				{
				    state = ReadState.Interactive;
				}

			    return nodetype;
			}


			/// <summary>See <see cref="XmlReader.MoveToElement"/></summary>
			public override bool MoveToElement() => false;

		    /// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/></summary>
			public override bool MoveToFirstAttribute() => false;

		    /// <summary>See <see cref="XmlReader.MoveToNextAttribute"/></summary>
			public override bool MoveToNextAttribute() => false;

		    /// <summary>See <see cref="XmlReader.Read"/></summary>
			public override bool Read()
			{
				if (state == ReadState.Initial)
				{
					state = ReadState.Interactive;
					return true;
				}
				if (state == ReadState.Interactive && nodetype == XmlNodeType.EndElement)
				{
					state = ReadState.EndOfFile;
					return false;
				}

				return false;
			}

			/// <summary>See <see cref="XmlReader.ReadAttributeValue"/></summary>
			public override bool ReadAttributeValue() => false;

		    /// <summary>See <see cref="XmlReader.ReadInnerXml"/></summary>
			public override string ReadInnerXml() => string.Empty;

		    /// <summary>See <see cref="XmlReader.ReadOuterXml"/></summary>
			public override string ReadOuterXml() => string.Empty;

		    /// <summary>See <see cref="XmlReader.Read"/></summary>
			public override void ResolveEntity()
			{
				// Not supported.
			}
		}
	}
}
