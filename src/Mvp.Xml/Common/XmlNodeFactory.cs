using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Creates <see cref="XmlNode"/> wrapper instances 
	/// for different XML APIs, for use in XML serialization.
	/// </summary>
	/// <remarks>
	/// <see cref="XmlNode"/> instances returned by this factory only 
	/// support the <see cref="XmlNode.WriteTo"/> and <see cref="XmlNode.WriteContentTo"/> 
	/// methods, as they are intended for use only for serialization, and to avoid 
	/// <see cref="XmlDocument"/> loading for fast performance. All other members 
	/// will throw an <see cref="NotSupportedException"/>.
	/// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
	/// See: http://weblogs.asp.net/cazzu/archive/2004/05/31/144922.aspx and 
	/// http://weblogs.asp.net/cazzu/posts/XmlMessagePerformance.aspx.
	/// </remarks>
	public class XmlNodeFactory
	{
		private XmlNodeFactory() { }

	    /// <summary>
		/// Creates an <see cref="XmlNode"/> wrapper for any object, 
		/// to be serialized through the <see cref="XmlSerializer"/>.
		/// </summary>
		/// <param name="value">The object to wrap.</param>
		/// <returns>A node that can only be used for XML serialization.</returns>
		public static XmlNode Create(object value) => new ObjectNode(value);

	    /// <summary>
		/// Creates an <see cref="XmlNode"/> serializable 
		/// wrapper for an <see cref="XPathNavigator"/>.
		/// </summary>
		/// <param name="navigator">The navigator to wrap.</param>
		/// <returns>A node that can only be used for XML serialization.</returns>
		public static XmlNode Create(XPathNavigator navigator) => new XPathNavigatorNode(navigator);

	    /// <summary>
		/// Creates an <see cref="XmlNode"/> serializable 
		/// wrapper for an <see cref="XmlReader"/>.
		/// </summary>
		/// <param name="reader">The reader to wrap.</param>
		/// <returns>A node that can only be used for XML serialization.</returns>
		/// <remarks>
		/// After serialization, the reader is automatically closed.
		/// </remarks>
		public static XmlNode Create(XmlReader reader) => Create(reader, false);

	    /// <summary>
		/// Creates an <see cref="XmlDocument"/> serializable 
		/// wrapper for an <see cref="XPathNavigator"/>.
		/// </summary>
		/// <param name="reader">The reader to wrap.</param>
		/// <param name="defaultAttrs">Whether default attributes should be serialized.</param>
		/// <returns>A document that can only be used for XML serialization.</returns>
		/// <remarks>
		/// After serialization, the reader is automatically closed.
		/// </remarks>
		public static XmlNode Create(XmlReader reader, bool defaultAttrs) => new XmlReaderNode(reader, defaultAttrs);

	    private abstract class SerializableNode : XmlElement
		{
		    protected SerializableNode() : base("", "dummy", "", new XmlDocument()) { }

			public override XmlNode AppendChild(XmlNode newChild) => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlAttributeCollection Attributes => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string BaseURI => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNodeList ChildNodes => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode Clone() => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode CloneNode(bool deep) => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode FirstChild => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string GetNamespaceOfPrefix(string prefix) => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string GetPrefixOfNamespace(string namespaceUri) => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override bool HasChildNodes => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string InnerText
			{
				get => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		        set => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		    }

			public override string InnerXml
			{
				get => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
			    set => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
			}

			public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild) => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild) => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override bool IsReadOnly => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode LastChild => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string LocalName => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string Name => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string NamespaceURI => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode NextSibling => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNodeType NodeType => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override void Normalize() => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string OuterXml => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlDocument OwnerDocument => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode ParentNode => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string Prefix
			{
				get => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		        set => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		    }

			public override XmlNode PrependChild(XmlNode newChild) => 
			    throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode PreviousSibling => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override void RemoveAll() => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode RemoveChild(XmlNode oldChild) => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild) => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override bool Supports(string feature, string version) => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlElement this[string localname, string ns] => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override XmlElement this[string name] => 
		        throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);

		    public override string Value
			{
				get => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		        set => throw new NotSupportedException(Properties.Resources.XmlDocumentFactory_NotImplementedDOM);
		    }

			public override void WriteContentTo(XmlWriter w) => WriteTo(w);

		    public abstract override void WriteTo(XmlWriter w);
		}

	    private class XPathNavigatorNode : SerializableNode
		{
			private readonly XPathNavigator navigator;

			public XPathNavigatorNode(XPathNavigator navigator)
			{
				this.navigator = navigator;
			}

			public override void WriteTo(XmlWriter w)
			{
				w.WriteNode(navigator.ReadSubtree(), false);
			}
		}

	    private class XmlReaderNode : SerializableNode
		{
			private readonly XmlReader reader;
			private readonly bool defaultAttrs;

			public XmlReaderNode(XmlReader reader, bool defaultAttrs)
			{
				this.reader = reader;
				this.reader.MoveToContent();
				this.defaultAttrs = defaultAttrs;
			}

			public override void WriteTo(XmlWriter w)
			{
				w.WriteNode(reader, defaultAttrs);
				reader.Close();
			}
		}

	    private class ObjectNode : SerializableNode
		{
			private readonly object serializableObject;

			public ObjectNode(object serializableObject)
			{
				this.serializableObject = serializableObject;
			}

			public override void WriteTo(XmlWriter w)
			{
				XmlSerializer ser = new XmlSerializer(serializableObject.GetType());
				ser.Serialize(w, serializableObject);
			}
		}
	}
}
