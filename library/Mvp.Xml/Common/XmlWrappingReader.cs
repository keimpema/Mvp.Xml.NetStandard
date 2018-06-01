using System;
using System.Xml;

namespace Mvp.Xml.Common
{
	/// <summary>
	/// Base <see cref="XmlReader"/> that can be use to create new readers by 
	/// wrapping existing ones.
	/// </summary>
	/// <remarks>
	/// Supports <see cref="IXmlLineInfo"/> if the underlying reader supports it.
	/// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a>.</para>
	/// </remarks>
	public abstract class XmlWrappingReader : XmlReader, IXmlLineInfo
	{
	    private XmlReader baseReader;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlWrappingReader"/>.
		/// </summary>
		/// <param name="baseReader">The underlying reader this instance will wrap.</param>
		protected XmlWrappingReader(XmlReader baseReader)
		{
			Guard.ArgumentNotNull(baseReader, "baseReader");
			this.baseReader = baseReader;
		}

		/// <summary>
		/// Gets or sets the underlying reader this instance is wrapping.
		/// </summary>
		protected XmlReader BaseReader
		{
			get => baseReader;
		    set
			{
				Guard.ArgumentNotNull(value, "value");
				baseReader = value;
			}
		}

		/// <summary>
		/// See <see cref="XmlReader.CanReadBinaryContent"/>.
		/// </summary>
		public override bool CanReadBinaryContent => baseReader.CanReadBinaryContent;

	    /// <summary>
		/// See <see cref="XmlReader.CanReadValueChunk"/>.
		/// </summary>
		public override bool CanReadValueChunk => baseReader.CanReadValueChunk;

	    /// <summary>
		/// See <see cref="XmlReader.CanResolveEntity"/>.
		/// </summary>
		public override bool CanResolveEntity => baseReader.CanResolveEntity;

	    /// <summary>
		/// See <see cref="XmlReader.Dispose"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (ReadState != ReadState.Closed)
			{
				Close();
			}

			((IDisposable)baseReader).Dispose();
		}

		/// <summary>
		/// See <see cref="XmlReader.Read"/>.
		/// </summary>
		public override bool Read() { return baseReader.Read(); }

		/// <summary>
		/// See <see cref="XmlReader.Close"/>.
		/// </summary>
		public override void Close() { baseReader.Close(); }

		/// <summary>
		/// See <see cref="XmlReader.GetAttribute(int)"/>.
		/// </summary>
		public override string GetAttribute(int i) { return baseReader.GetAttribute(i); }

		/// <summary>
		/// See <see cref="XmlReader.GetAttribute(string)"/>.
		/// </summary>
		public override string GetAttribute(string name) { return baseReader.GetAttribute(name); }

		/// <summary>
		/// See <see cref="XmlReader.GetAttribute(string, string)"/>.
		/// </summary>
		public override string GetAttribute(string localName, string namespaceUri) { { return baseReader.GetAttribute(localName, namespaceUri); } }

		/// <summary>
		/// See <see cref="XmlReader.LookupNamespace"/>.
		/// </summary>
		public override string LookupNamespace(string prefix) { return baseReader.LookupNamespace(prefix); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToAttribute(int)"/>.
		/// </summary>
		public override void MoveToAttribute(int i) { baseReader.MoveToAttribute(i); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToAttribute(string)"/>.
		/// </summary>
		public override bool MoveToAttribute(string name) { return baseReader.MoveToAttribute(name); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToAttribute(string, string)"/>.
		/// </summary>
		public override bool MoveToAttribute(string localName, string namespaceUri) { return baseReader.MoveToAttribute(localName, namespaceUri); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToElement"/>.
		/// </summary>
		public override bool MoveToElement() { return baseReader.MoveToElement(); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToFirstAttribute"/>.
		/// </summary>
		public override bool MoveToFirstAttribute() { return baseReader.MoveToFirstAttribute(); }

		/// <summary>
		/// See <see cref="XmlReader.MoveToNextAttribute"/>.
		/// </summary>
		public override bool MoveToNextAttribute() { return baseReader.MoveToNextAttribute(); }

		/// <summary>
		/// See <see cref="XmlReader.ReadAttributeValue"/>.
		/// </summary>
		public override bool ReadAttributeValue() { return baseReader.ReadAttributeValue(); }

		/// <summary>
		/// See <see cref="XmlReader.ResolveEntity"/>.
		/// </summary>
		public override void ResolveEntity() { baseReader.ResolveEntity(); }

		/// <summary>
		/// See <see cref="XmlReader.AttributeCount"/>.
		/// </summary>
		public override int AttributeCount => baseReader.AttributeCount;

	    /// <summary>
		/// See <see cref="XmlReader.BaseURI"/>.
		/// </summary>
		public override string BaseURI => baseReader.BaseURI;

	    /// <summary>
		/// See <see cref="XmlReader.Depth"/>.
		/// </summary>
		public override int Depth => baseReader.Depth;

	    /// <summary>
		/// See <see cref="XmlReader.EOF"/>.
		/// </summary>
		public override bool EOF => baseReader.EOF;

	    /// <summary>
		/// See <see cref="XmlReader.HasValue"/>.
		/// </summary>
		public override bool HasValue => baseReader.HasValue;

	    /// <summary>
		/// See <see cref="XmlReader.IsDefault"/>.
		/// </summary>
		public override bool IsDefault => baseReader.IsDefault;

	    /// <summary>
		/// See <see cref="XmlReader.IsEmptyElement"/>.
		/// </summary>
		public override bool IsEmptyElement => baseReader.IsEmptyElement;

	    /// <summary>
		/// See <see cref="XmlReader.this[int]"/>.
		/// </summary>
		public override string this[int i] => baseReader[i];

	    /// <summary>
		/// See <see cref="XmlReader.this[string]"/>.
		/// </summary>
		public override string this[string name] => baseReader[name];

	    /// <summary>
		/// See <see cref="XmlReader.this[string, string]"/>.
		/// </summary>
		public override string this[string name, string namespaceUri] => baseReader[name, namespaceUri];

	    /// <summary>
		/// See <see cref="XmlReader.LocalName"/>.
		/// </summary>
		public override string LocalName => baseReader.LocalName;

	    /// <summary>
		/// See <see cref="XmlReader.Name"/>.
		/// </summary>
		public override string Name => baseReader.Name;

	    /// <summary>
		/// See <see cref="XmlReader.NamespaceURI"/>.
		/// </summary>
		public override string NamespaceURI => baseReader.NamespaceURI;

	    /// <summary>
		/// See <see cref="XmlReader.NameTable"/>.
		/// </summary>
		public override XmlNameTable NameTable => baseReader.NameTable;

	    /// <summary>
		/// See <see cref="XmlReader.NodeType"/>.
		/// </summary>
		public override XmlNodeType NodeType => baseReader.NodeType;

	    /// <summary>
		/// See <see cref="XmlReader.Prefix"/>.
		/// </summary>
		public override string Prefix => baseReader.Prefix;

	    /// <summary>
		/// See <see cref="XmlReader.QuoteChar"/>.
		/// </summary>
		public override char QuoteChar => baseReader.QuoteChar;

	    /// <summary>
		/// See <see cref="XmlReader.ReadState"/>.
		/// </summary>
		public override ReadState ReadState => baseReader.ReadState;

	    /// <summary>
		/// See <see cref="XmlReader.Value"/>.
		/// </summary>
		public override string Value => baseReader.Value;

	    /// <summary>
		/// See <see cref="XmlReader.XmlLang"/>.
		/// </summary>
		public override string XmlLang => baseReader.XmlLang;

	    /// <summary>
		/// See <see cref="XmlReader.XmlSpace"/>.
		/// </summary>
		public override XmlSpace XmlSpace => baseReader.XmlSpace;

	    /// <summary>
		/// See <see cref="XmlReader.ReadValueChunk"/>.
		/// </summary>
		public override int ReadValueChunk(char[] buffer, int index, int count) { return baseReader.ReadValueChunk(buffer, index, count); }

	    /// <summary>
		/// See <see cref="IXmlLineInfo.HasLineInfo"/>.
		/// </summary>
		public bool HasLineInfo()
		{
		    if (baseReader is IXmlLineInfo info)
			{
				return info.HasLineInfo();
			}

			return false;
		}

		/// <summary>
		/// See <see cref="IXmlLineInfo.LineNumber"/>.
		/// </summary>
		public int LineNumber
		{
			get
			{
			    if (baseReader is IXmlLineInfo info)
				{
					return info.LineNumber;
				}

				return 0;
			}
		}

		/// <summary>
		/// See <see cref="IXmlLineInfo.LinePosition"/>.
		/// </summary>
		public int LinePosition
		{
			get
			{
			    if (baseReader is IXmlLineInfo info)
				{
					return info.LinePosition;
				}

				return 0;
			}
		}
	}
}
