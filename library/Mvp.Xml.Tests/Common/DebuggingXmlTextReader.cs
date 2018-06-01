#region using

using System;
using System.IO;
using System.Xml;

#endregion using 

namespace Mvp.Xml.Tests
{
	internal class DebuggingXmlTextReader : XmlTextReader
	{
		XmlReader _reader;
		public DebuggingXmlTextReader(Stream input) : base(new StringReader(String.Empty))
		{
			_reader = new XmlTextReader(input);
		}
		public DebuggingXmlTextReader(TextReader input) : base(new StringReader(String.Empty))
		{
			_reader = new XmlTextReader(input);
		}
		public DebuggingXmlTextReader(XmlReader input) : base(new StringReader(String.Empty))
		{
			_reader = input;
		}

		public override int AttributeCount
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("AttributeCount = " + _reader.AttributeCount);
				return _reader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("BaseURI = " + _reader.BaseURI);
				return _reader.BaseURI;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("CanResolveEntity = " + _reader.CanResolveEntity);
				return _reader.CanResolveEntity;
			}
		}

		public override void Close()
		{
			System.Diagnostics.Debug.WriteLine("Close");
			_reader.Close();
		}

		public override int Depth
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("Depth = " + _reader.Depth);
				return _reader.Depth;
			}
		}

		public override bool EOF
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("EOF = " + _reader.EOF);
				return _reader.EOF;
			}
		}

		public override string GetAttribute(int i)
		{
			System.Diagnostics.Debug.WriteLine("GetAttribute(" + i + ")");
			return _reader.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			System.Diagnostics.Debug.WriteLine("GetAttribute(" + name + ")");
			return _reader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			System.Diagnostics.Debug.WriteLine("GetAttribute(" + name + "), " + namespaceURI + ")");
			return _reader.GetAttribute(name, namespaceURI);
		}

		public override bool HasAttributes
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("HasAttributes = " + _reader.HasAttributes);
				return _reader.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("HasValue = " + _reader.HasValue);
				return _reader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("IsDefault = " + _reader.IsDefault);
				return _reader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("IsEmptyElement = " + _reader.IsEmptyElement);
				return _reader.IsEmptyElement;
			}
		}

		public override bool IsStartElement()
		{
			System.Diagnostics.Debug.WriteLine("IsStartElement()");
			return _reader.IsStartElement ();
		}

		public override bool IsStartElement(string localname, string ns)
		{
			System.Diagnostics.Debug.WriteLine("IsStartElement(" + localname + ", " + ns + ")");
			return _reader.IsStartElement (localname, ns);
		}

		public override bool IsStartElement(string name)
		{
			System.Diagnostics.Debug.WriteLine("IsStartElement(" + name + ")");
			return _reader.IsStartElement (name);
		}

		public override string LocalName
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("LocalName = " + _reader.LocalName);
				return _reader.LocalName;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			System.Diagnostics.Debug.WriteLine("LookupNamespace(" + prefix + ")");
			return _reader.Prefix;
		}

		public override void MoveToAttribute(int i)
		{
			System.Diagnostics.Debug.WriteLine("MoveToAttribute(" + i + ")");
			_reader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			System.Diagnostics.Debug.WriteLine("MoveToAttribute(" + name + ")");
			return _reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			System.Diagnostics.Debug.WriteLine("MoveToAttribute(" + name + ", " + ns + ")");
			return _reader.MoveToAttribute(name, ns);
		}

		public override XmlNodeType MoveToContent()
		{
			System.Diagnostics.Debug.WriteLine("MoveToContent()");
			return _reader.MoveToContent();
		}

		public override bool MoveToElement()
		{
			System.Diagnostics.Debug.WriteLine("MoveToElement()");
			return _reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			System.Diagnostics.Debug.WriteLine("MoveToFirstAttribute()");
			return _reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			System.Diagnostics.Debug.WriteLine("MoveToNextAttribute()");
			return _reader.MoveToNextAttribute();
		}

		public override string Name
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("Name = " + _reader.Name);
				return _reader.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("NamespaceURI = " + _reader.NamespaceURI);
				return _reader.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("NameTable");
				return _reader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("NodeType = " + _reader.NodeType);
				return _reader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("Prefix = " + _reader.Prefix);
				return _reader.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("QuoteChar = " + _reader.QuoteChar);
				return _reader.QuoteChar;
			}
		}

		public override bool Read()
		{
			System.Diagnostics.Debug.WriteLine("Read()");
			return _reader.Read();
		}

		public override bool ReadAttributeValue()
		{
			System.Diagnostics.Debug.WriteLine("ReadAttributeValue()");
			return _reader.ReadAttributeValue();
		}

		public override string ReadElementString()
		{
			System.Diagnostics.Debug.WriteLine("ReadElementString()");
			return _reader.ReadElementString();
		}

		public override string ReadElementString(string localname, string ns)
		{
			System.Diagnostics.Debug.WriteLine("ReadElementString(" + localname + ", " + ns + ")");
			return _reader.ReadElementString(localname, ns);
		}

		public override string ReadElementString(string name)
		{
			System.Diagnostics.Debug.WriteLine("ReadElementString(" + name + ")");
			return _reader.ReadElementString(name);
		}

		public override void ReadEndElement()
		{
			System.Diagnostics.Debug.WriteLine("ReadEndElement()");
			_reader.ReadEndElement();
		}

		public override string ReadInnerXml()
		{
			System.Diagnostics.Debug.WriteLine("ReadInnerXml()");
			return _reader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			System.Diagnostics.Debug.WriteLine("ReadOuterXml()");
			return _reader.ReadOuterXml();
		}

		public override void ReadStartElement()
		{
			System.Diagnostics.Debug.WriteLine("ReadStartElement()");
			_reader.ReadStartElement();
		}

		public override void ReadStartElement(string localname, string ns)
		{
			System.Diagnostics.Debug.WriteLine("ReadStartElement(" + localname + ", " + ns + ")");
			_reader.ReadStartElement(localname, ns);
		}

		public override void ReadStartElement(string name)
		{
			System.Diagnostics.Debug.WriteLine("ReadStartElement(" + name + ")");
			_reader.ReadStartElement(name);
		}

		public override ReadState ReadState
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("ReadState = " + _reader.ReadState);
				return _reader.ReadState;
			}
		}

		public override string ReadString()
		{
			System.Diagnostics.Debug.WriteLine("ReadString()");
			return _reader.ReadString();
		}

		public override void ResolveEntity()
		{
			System.Diagnostics.Debug.WriteLine("ResolveEntity()");
			_reader.ResolveEntity();
		}

		public override void Skip()
		{
			System.Diagnostics.Debug.WriteLine("Skip()");
			_reader.Skip();
		}

		public override string this[int i]
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("this[" + i + "]");
				return base[i];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("this[" + name + ", " + namespaceURI + "]");
				return base[name, namespaceURI];
			}
		}

		public override string this[string name]
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("this[" + name + "]");
				return base[name];
			}
		}

		public override string Value
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("Value = " + _reader.Value);
				return  _reader.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("XmlLang = " + _reader.XmlLang);
				return _reader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				System.Diagnostics.Debug.WriteLine("XmlSpace = " + _reader.XmlSpace);
				return _reader.XmlSpace;
			}
		}
	}
}