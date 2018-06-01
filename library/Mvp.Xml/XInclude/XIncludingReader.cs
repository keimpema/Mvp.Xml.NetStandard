using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Net;
using System.Text;
using System.Security;
using System.Collections.Generic;
using System.Reflection;

using Mvp.Xml.Common;
using Mvp.Xml.XPointer;
using System.Globalization;

namespace Mvp.Xml.XInclude
{
	/// <summary>
	/// <c>XIncludingReader</c> class implements streamable subset of the
	/// XInclude 1.0 in a fast, non-caching, forward-only fashion.
	/// To put it another way <c>XIncludingReader</c> is XML Base and XInclude 1.0 aware
	/// <see cref="XmlReader"/>.
	/// </summary>
    /// <remarks><para>See <a href="http://www.codeplex.com/MVPXML/Wiki/View.aspx?title=XInclude.NET">XInclude.NET homepage</a> for more info.</para>    
	/// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
	/// </remarks>
	public class XIncludingReader : XmlReader, IXmlLineInfo
	{
	    //XInclude keywords
		private readonly XIncludeKeywords keywords;
		//Current reader
		private XmlReader reader;
		//Stack of readers
		private readonly Stack<XmlReader> readers;
		//Top base URI
		private readonly Uri topBaseUri;
		//Top-level included item flag
		private bool topLevel;
		//A top-level included element has been included already
		private bool gotTopIncludedElem;
		//At least one element has been returned by the reader
		private bool gotElement;
		//Internal state
		private XIncludingReaderState state;
		//Name table
		private readonly XmlNameTable nameTable;
		//Whitespace handling
	    //Emit relative xml:base URIs
	    //Current fallback state
		private FallbackState fallbackState;
		//Previous fallback state (imagine enclosed deep xi:fallback/xi:include tree)
		private FallbackState prevFallbackState;
		//XmlResolver to resolve URIs
		XmlResolver xmlResolver;
		//Expose text inclusions as CDATA
	    //Top-level included item has different xml:lang
		private bool differentLang;
		//Acquired infosets cache
		private static IDictionary<string, WeakReference> cache;
        //Real xml:base attribute index
        int realXmlBaseIndex = -1;

	    /// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified underlying <c>XmlReader</c> reader.
		/// </summary>
		/// <param name="reader">Underlying reader to read from</param>        
		public XIncludingReader(XmlReader reader)
		{
		    if (reader is XmlTextReader xtr)
			{
//#pragma warning disable 0618
                //XmlValidatingReader vr = new XmlValidatingReader(reader);
                //vr.ValidationType = ValidationType.None;
                //vr.EntityHandling = EntityHandling.ExpandEntities;
                //vr.ValidationEventHandler += new ValidationEventHandler(
                //    ValidationCallback);
                //_whiteSpaceHandling = xtr.WhitespaceHandling;
                //_reader = vr;                                
			    XmlReaderSettings s = new XmlReaderSettings
			    {
			        DtdProcessing = DtdProcessing.Parse,
			        ValidationType = ValidationType.None
			    };
			    //s.ProhibitDtd = false;
			    s.ValidationEventHandler += ValidationCallback;
                if (xtr.WhitespaceHandling == WhitespaceHandling.Significant) 
                {
                    s.IgnoreWhitespace = true;
                }
                this.reader = Create(reader, s);
//#pragma warning restore 0618
			}
			else
			{
				this.reader = reader;
			}

			nameTable = reader.NameTable;
			keywords = new XIncludeKeywords(NameTable);
			if (this.reader.BaseURI != "")
			{
			    topBaseUri = new Uri(this.reader.BaseURI);
			}
			else
			{
				MakeRelativeBaseUri = false;
				topBaseUri = new Uri(Assembly.GetExecutingAssembly().Location);
			}
			readers = new Stack<XmlReader>();
			state = XIncludingReaderState.Default;
		}

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL.
		/// </summary>
		/// <param name="url">Document location.</param>
		public XIncludingReader(string url)
			: this(new XmlBaseAwareXmlReader(url)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL and resolver.
		/// </summary>
		/// <param name="url">Document location.</param>
		/// <param name="resolver">Resolver to acquire external resources.</param>
		public XIncludingReader(string url, XmlResolver resolver)
			: this(new XmlBaseAwareXmlReader(url, resolver)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL and nametable.
		/// </summary>
		/// <param name="url">Document location.</param>
		/// <param name="nt">Name table.</param>
		public XIncludingReader(string url, XmlNameTable nt)
			:
			this(new XmlBaseAwareXmlReader(url, nt)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified <c>TextReader</c> reader.
		/// </summary>
		/// <param name="reader"><c>TextReader</c>.</param>
		public XIncludingReader(TextReader reader)
			: this(new XmlBaseAwareXmlReader(reader)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL and <c>TextReader</c> reader.
		/// </summary>
		/// <param name="reader"><c>TextReader</c>.</param>
		/// <param name="url">Source document's URL</param>
		public XIncludingReader(string url, TextReader reader)
			: this(new XmlBaseAwareXmlReader(url, reader)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified <c>TextReader</c> reader and nametable.
		/// </summary>
		/// <param name="reader"><c>TextReader</c>.</param>
		/// <param name="nt">Nametable.</param>
		public XIncludingReader(TextReader reader, XmlNameTable nt)
			:
			this(new XmlBaseAwareXmlReader(reader, nt)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL, <c>TextReader</c> reader and nametable.
		/// </summary>
		/// <param name="reader"><c>TextReader</c>.</param>
		/// <param name="nt">Nametable.</param>
		/// <param name="url">Source document's URI</param>
		public XIncludingReader(string url, TextReader reader, XmlNameTable nt)
			:
			this(new XmlBaseAwareXmlReader(url, reader, nt)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified <c>Stream</c>.
		/// </summary>
		/// <param name="input"><c>Stream</c>.</param>
		public XIncludingReader(Stream input)
			: this(new XmlBaseAwareXmlReader(input)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL and <c>Stream</c>.
		/// </summary>
		/// <param name="input"><c>Stream</c>.</param>
		/// <param name="url">Source document's URL</param>
		public XIncludingReader(string url, Stream input)
			: this(new XmlBaseAwareXmlReader(url, input)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL, <c>Stream</c> and resolver.
		/// </summary>
		/// <param name="input"><c>Stream</c>.</param>
		/// <param name="url">Source document's URL</param>
		/// <param name="resolver">Resolver to acquire external resources.</param>
		public XIncludingReader(string url, Stream input, XmlResolver resolver)
			: this(new XmlBaseAwareXmlReader(url, input, resolver)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified <c>Stream</c> and nametable.
		/// </summary>
		/// <param name="input"><c>Stream</c>.</param>
		/// <param name="nt">Nametable</param>
		public XIncludingReader(Stream input, XmlNameTable nt)
			: this(new XmlBaseAwareXmlReader(input, nt)) { }

		/// <summary>
		/// Creates new instance of <c>XIncludingReader</c> class with
		/// specified URL, <c>Stream</c> and nametable.
		/// </summary>
		/// <param name="input"><c>Stream</c>.</param>
		/// <param name="nt">Nametable</param>
		/// <param name="url">Source document's URL</param>
		public XIncludingReader(string url, Stream input, XmlNameTable nt)
			: this(new XmlBaseAwareXmlReader(url, input, nt)) { }

	    /// <summary>See <see cref="XmlReader.AttributeCount"/></summary>
		public override int AttributeCount
		{
			get
			{
				if (topLevel)
				{
					int ac = reader.AttributeCount;
					if (reader.GetAttribute(keywords.XmlBase) == null)
					{
					    ac++;
					}

				    if (differentLang)
				    {
				        ac++;
				    }

				    return ac;
				}
				else
				{
				    return reader.AttributeCount;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.BaseURI"/></summary>
		public override string BaseURI => reader.BaseURI;

	    /// <summary>See <see cref="XmlReader.HasValue"/></summary>
		public override bool HasValue
		{
			get
			{
				if (state == XIncludingReaderState.Default)
				{
				    return reader.HasValue;
				}
				else
				{
				    return true;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.IsDefault"/></summary>
		public override bool IsDefault
		{
			get
			{
				if (state == XIncludingReaderState.Default)
				{
				    return reader.IsDefault;
				}
				else
				{
				    return false;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.Name"/></summary>
		public override string Name
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
						return keywords.XmlBase;
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return string.Empty;
					case XIncludingReaderState.ExposingXmlLangAttr:
						return keywords.XmlLang;
					default:
						return reader.Name;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.LocalName"/></summary>
		public override string LocalName
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
						return keywords.Base;
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return string.Empty;
					case XIncludingReaderState.ExposingXmlLangAttr:
						return keywords.Lang;
					default:
						return reader.LocalName;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.NamespaceURI"/></summary>
		public override string NamespaceURI
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlLangAttr:
						return keywords.XmlNamespace;
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return string.Empty;
					default:
						return reader.NamespaceURI;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.NameTable"/></summary>
		public override XmlNameTable NameTable => nameTable;

	    /// <summary>See <see cref="XmlReader.NodeType"/></summary>
		public override XmlNodeType NodeType
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlLangAttr:
						return XmlNodeType.Attribute;
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return XmlNodeType.Text;
					default:
						return reader.NodeType;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.Prefix"/></summary>
		public override string Prefix
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlLangAttr:
						return keywords.Xml;
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return string.Empty;
					default:
						return reader.Prefix;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.QuoteChar"/></summary>
		public override char QuoteChar
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlLangAttr:
						return '"';
					default:
						return reader.QuoteChar;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.Close"/></summary>
		public override void Close()
		{
		    reader?.Close();

		    //Close all readers in the stack
			while (readers.Count > 0)
			{
				reader = readers.Pop();
			    reader?.Close();
			}
		}

		/// <summary>See <see cref="XmlReader.Depth"/></summary>
		public override int Depth
		{
			get
			{
				if (readers.Count == 0)
				{
				    return reader.Depth;
				}
				//TODO: that might be way ineffective
				return readers.Peek().Depth + reader.Depth;
			}
		}

		/// <summary>See <see cref="XmlReader.EOF"/></summary>
		public override bool EOF => reader.EOF;

	    /// <summary>See <see cref="XmlReader.GetAttribute(int)"/></summary>
		public override string GetAttribute(int i)
		{
            if (topLevel)
            {
                int ac = reader.AttributeCount;
                if (i < ac)
                {
                    if (i == realXmlBaseIndex)
                    {
                        //case 1: it's real xml:base
                        return GetBaseUri();
                    }
                    else
                    {
                        //case 2: it's real attribute and it's not xml:base
                        return reader.GetAttribute(i);                     
                    }
                }
                else
                {
                    if (i == ac)
                    {
                        //case 3: it's virtual xml:base - it comes first
                        return GetBaseUri();
                    }
                    else
                    {
                        //case 4: it's virtual xml:lang - it comes last
                        return reader.XmlLang;
                    }
                }                                                               
            }
            else
            {
                return reader.GetAttribute(i);
            }
		}       

		/// <summary>See <see cref="XmlReader.GetAttribute(string)"/></summary>
		public override string GetAttribute(string name)
		{
			if (topLevel)
			{
                if (XIncludeKeywords.Equals(name, keywords.XmlBase))
                {
                    return GetBaseUri();
                }
                else if (XIncludeKeywords.Equals(name, keywords.XmlLang))
                {
                    return reader.XmlLang;
                }
			}
			return reader.GetAttribute(name);
		}

		/// <summary>See <see cref="XmlReader.GetAttribute(string, string)"/></summary>
		public override string GetAttribute(string name, string namespaceUri)
		{
			if (topLevel)
			{
                if (XIncludeKeywords.Equals(name, keywords.Base) &&
                    XIncludeKeywords.Equals(namespaceUri, keywords.XmlNamespace))
                {
                    return GetBaseUri();
                }
                else if (XIncludeKeywords.Equals(name, keywords.Lang) &&
                    XIncludeKeywords.Equals(namespaceUri, keywords.XmlNamespace))
                {
                    return reader.XmlLang;
                }
			}
			return reader.GetAttribute(name, namespaceUri);
		}

		/// <summary>See <see cref="XmlReader.IsEmptyElement"/></summary>
		public override bool IsEmptyElement => reader.IsEmptyElement;

	    /// <summary>See <see cref="XmlReader.LookupNamespace"/></summary>
		public override string LookupNamespace(string prefix)
		{
			return reader.LookupNamespace(prefix);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(int)"/></summary>
		public override void MoveToAttribute(int i)
		{
            if (topLevel)
            {                
                if (i >= reader.AttributeCount || i == realXmlBaseIndex)
                {
                    state = XIncludingReaderState.ExposingXmlBaseAttr;
                }
                else
                {
                    reader.MoveToAttribute(i);
                }                
            }
            else
            {
                reader.MoveToAttribute(i);
            }
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(string)"/></summary>
		public override bool MoveToAttribute(string name)
		{
			if (topLevel)
			{
				if (XIncludeKeywords.Equals(name, keywords.XmlBase))
				{
					state = XIncludingReaderState.ExposingXmlBaseAttr;
					return true;
				}
				else if (XIncludeKeywords.Equals(name, keywords.XmlLang))
				{
					state = XIncludingReaderState.ExposingXmlLangAttr;
					return true;
				}
			}
			return reader.MoveToAttribute(name);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute(string, string)"/></summary>
		public override bool MoveToAttribute(string name, string ns)
		{
			if (topLevel)
			{
				if (XIncludeKeywords.Equals(name, keywords.Base) &&
					XIncludeKeywords.Equals(ns, keywords.XmlNamespace))
				{
					state = XIncludingReaderState.ExposingXmlBaseAttr;
					return true;
				}
				else if (XIncludeKeywords.Equals(name, keywords.Lang) &&
					XIncludeKeywords.Equals(ns, keywords.XmlNamespace))
				{
					state = XIncludingReaderState.ExposingXmlLangAttr;
					return true;
				}
			}
			return reader.MoveToAttribute(name, ns);
		}

		/// <summary>See <see cref="XmlReader.MoveToElement"/></summary>
		public override bool MoveToElement()
		{
			return reader.MoveToElement();
		}

		/// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/></summary>
		public override bool MoveToFirstAttribute()
		{
		    if (!topLevel)
		    {
		        return reader.MoveToFirstAttribute();
		    }

		    if (reader.MoveToFirstAttribute())
		    {
		        //it might be xml:base or xml:lang
		        if (reader.Name == keywords.XmlBase ||
		            reader.Name == keywords.XmlLang)
		            //omit them - we expose virtual ones at the end
		        {
		            return MoveToNextAttribute();
		        }

		        return true;
		    }

		    // No attrs? Expose xml:base
		    state = XIncludingReaderState.ExposingXmlBaseAttr;
		    return true;

		}

		/// <summary>See <see cref="XmlReader.MoveToNextAttribute"/></summary>
		public override bool MoveToNextAttribute()
		{
			if (topLevel)
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
						//Exposing xml:base already - switch to xml:lang                                                                            
						if (differentLang)
						{
							state = XIncludingReaderState.ExposingXmlLangAttr;
							return true;
						}
						else
						{
							//No need for xml:lang, stop
							state = XIncludingReaderState.Default;
							return false;
						}
					case XIncludingReaderState.ExposingXmlLangAttr:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						//Exposing xml:lang already - that's a last one
						state = XIncludingReaderState.Default;
						return false;
					default:
						//1+ attrs, default mode
						if (reader.MoveToNextAttribute())
						{
						    //Still real attributes - it might be xml:base or xml:lang
							if (reader.Name == keywords.XmlBase ||
								reader.Name == keywords.XmlLang)
								//omit them - we expose virtual ones at the end
							{
							    return MoveToNextAttribute();
							}

						    return true;
						}
						else
						{
							//No more attrs - expose virtual xml:base                                
							state = XIncludingReaderState.ExposingXmlBaseAttr;
							return true;
						}
				}

			}
			else
			{
			    return reader.MoveToNextAttribute();
			}
		}

		/// <summary>See <see cref="XmlReader.ReadAttributeValue"/></summary>
		public override bool ReadAttributeValue()
		{
			switch (state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
					state = XIncludingReaderState.ExposingXmlBaseAttrValue;
					return true;
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
					return false;
				case XIncludingReaderState.ExposingXmlLangAttr:
					state = XIncludingReaderState.ExposingXmlLangAttrValue;
					return true;
				case XIncludingReaderState.ExposingXmlLangAttrValue:
					return false;
				default:
					return reader.ReadAttributeValue();
			}
		}

		/// <summary>See <see cref="XmlReader.ReadState"/></summary>
		public override ReadState ReadState => reader.ReadState;

	    /// <summary>See <see cref="XmlReader.this[int]"/></summary>
		public override string this[int i] => GetAttribute(i);

	    /// <summary>See <see cref="XmlReader.this[string]"/></summary>
		public override string this[string name] => GetAttribute(name);

	    /// <summary>See <see cref="XmlReader.this[string, string]"/></summary>
		public override string this[string name, string namespaceUri] => GetAttribute(name, namespaceUri);

	    /// <summary>See <see cref="XmlReader.ResolveEntity"/></summary>
		public override void ResolveEntity()
		{
			reader.ResolveEntity();
		}

		/// <summary>See <see cref="XmlReader.XmlLang"/></summary>
		public override string XmlLang => reader.XmlLang;

	    /// <summary>See <see cref="XmlReader.XmlSpace"/></summary>
		public override XmlSpace XmlSpace => reader.XmlSpace;

	    /// <summary>See <see cref="XmlReader.Value"/></summary>
		public override string Value
		{
			get
			{
				switch (state)
				{
					case XIncludingReaderState.ExposingXmlBaseAttr:
					case XIncludingReaderState.ExposingXmlBaseAttrValue:
                        return GetBaseUri();
					case XIncludingReaderState.ExposingXmlLangAttr:
					case XIncludingReaderState.ExposingXmlLangAttrValue:
						return reader.XmlLang;
					default:
						return reader.Value;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.ReadInnerXml"/></summary>
		public override string ReadInnerXml()
		{
			switch (state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
                    return GetBaseUri();
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
					return string.Empty;
				case XIncludingReaderState.ExposingXmlLangAttr:
					return reader.XmlLang;
				case XIncludingReaderState.ExposingXmlLangAttrValue:
					return string.Empty;
				default:
					if (NodeType == XmlNodeType.Element)
					{
						int depth = Depth;
						if (Read())
						{
							StringWriter sw = new StringWriter();
							XmlTextWriter xw = new XmlTextWriter(sw);
							while (Depth > depth)
							{
							    xw.WriteNode(this, false);
							}

						    xw.Close();
							return sw.ToString();
						}
						else
						{
						    return string.Empty;
						}
					}
					else if (NodeType == XmlNodeType.Attribute)
					{
						return Value;
					}
					else
					{
					    return string.Empty;
					}
			}
		}

		/// <summary>See <see cref="XmlReader.ReadOuterXml"/></summary>
		public override string ReadOuterXml()
		{
			switch (state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
					return @"xml:base="" + _reader.BaseURI + @""";
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
					return string.Empty;
				case XIncludingReaderState.ExposingXmlLangAttr:
					return @"xml:lang="" + _reader.XmlLang + @""";
				case XIncludingReaderState.ExposingXmlLangAttrValue:
					return string.Empty;
				default:
					if (NodeType == XmlNodeType.Element)
					{
						StringWriter sw = new StringWriter();
						XmlTextWriter xw = new XmlTextWriter(sw);
						xw.WriteNode(this, false);
						xw.Close();
						return sw.ToString();
					}
					else if (NodeType == XmlNodeType.Attribute)
					{
						return string.Format("{0}=\"{1}\"}", Name, Value);
					}
					else
					{
					    return string.Empty;
					}
			}
		}

		/// <summary>See <see cref="XmlReader.ReadString"/></summary>
		public override string ReadString()
		{
			switch (state)
			{
				case XIncludingReaderState.ExposingXmlBaseAttr:
					return string.Empty;
				case XIncludingReaderState.ExposingXmlBaseAttrValue:
                    return GetBaseUri();
				case XIncludingReaderState.ExposingXmlLangAttr:
					return string.Empty;
				case XIncludingReaderState.ExposingXmlLangAttrValue:
					return reader.XmlLang;
				default:
					return reader.ReadString();
			}
		}

		/// <summary>See <see cref="XmlReader.Read"/></summary>
		public override bool Read()
		{
            state = XIncludingReaderState.Default;
			//Read internal reader
			bool baseRead = reader.Read();
			if (baseRead)
			{
				//If we are including and including reader is at 0 depth - 
				//we are at a top level included item
				topLevel = (readers.Count > 0 && reader.Depth == 0);				                
				if (topLevel && reader.NodeType == XmlNodeType.Attribute)
					//Attempt to include an attribute or namespace node
				{
				    throw new AttributeOrNamespaceInIncludeLocationError(Properties.Resources.AttributeOrNamespaceInIncludeLocationError);
				}

			    if (topLevel && readers.Peek().Depth == 0 &&
					reader.NodeType == XmlNodeType.Element)
				{
					if (gotTopIncludedElem)
						//Attempt to include more than one element at the top level
					{
					    throw new MalformedXInclusionResultError(Properties.Resources.MalformedXInclusionResult);
					}
					else
					{
					    gotTopIncludedElem = true;
					}
				}
                if (topLevel)
                {
                    //Check if included item has different language
                    differentLang = AreDifferentLangs(reader.XmlLang, readers.Peek().XmlLang);
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        //Record real xml:base index
                        realXmlBaseIndex = -1;
                        int i = 0;
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == keywords.XmlBase)
                            {
                                realXmlBaseIndex = i;
                                break;
                            }
                            i++;
                        }
                        reader.MoveToElement();
                    }
                }
				switch (reader.NodeType)
				{
					case XmlNodeType.XmlDeclaration:
					case XmlNodeType.Document:
					case XmlNodeType.DocumentType:
					case XmlNodeType.DocumentFragment:
						//This stuff should not be included into resulting infoset,
						//but should be inclused into acquired infoset                        
						return readers.Count <= 0 || Read();
					case XmlNodeType.Element:
						//Check for xi:include
						if (IsIncludeElement())
						{
							//xi:include element found
							//Save current reader to possible fallback processing
							XmlReader current = reader;
							try
							{
								return ProcessIncludeElement();
							}
							catch (FatalException)
							{
								throw;
							}
							catch (Exception e)
							{
								//Let's be liberal - any exceptions other than fatal one 
								//should be treated as resource error
								//Console.WriteLine("Resource error has been detected: " + e.Message);
								//Start fallback processing
								if (!current.Equals(reader))
								{
									reader.Close();
									reader = current;
								}
								prevFallbackState = fallbackState;
								return ProcessFallback(reader.Depth, e);
							}
							//No, it's not xi:include, check it for xi:fallback    
						}
						else if (IsFallbackElement())
						{
							//Found xi:fallback not child of xi:include
						    if (reader is IXmlLineInfo li && li.HasLineInfo())
							{
								throw new XIncludeSyntaxError(string.Format(
									CultureInfo.CurrentCulture,
									Properties.Resources.FallbackNotChildOfIncludeLong,
									reader.BaseURI, li.LineNumber,
									li.LinePosition));
							}

						    throw new XIncludeSyntaxError(string.Format(
						        CultureInfo.CurrentCulture,
						        Properties.Resources.FallbackNotChildOfInclude,
						        reader.BaseURI));
						}
						else
						{
							gotElement = true;
							goto default;
						}
					case XmlNodeType.EndElement:
						//Looking for end of xi:fallback
						if (fallbackState.Fallbacking &&
							reader.Depth == fallbackState.FallbackDepth &&
							IsFallbackElement())
						{
							//End of fallback processing
							fallbackState.FallbackProcessed = true;
							//Now read other ignored content till </xi:fallback>
							return ProcessFallback(reader.Depth - 1, null);
						}
						else
						{
						    goto default;
						}
				    default:
						return true;
				}
			}

		    //No more input - finish possible xi:include processing
		    if (topLevel)
		    {
		        topLevel = false;
		    }

		    if (readers.Count > 0)
		    {
		        reader.Close();
		        //Pop previous reader
		        reader = readers.Pop();
		        //Successful include - skip xi:include content
		        if (!reader.IsEmptyElement)
		        {
		            CheckAndSkipContent();
		        }

		        return Read();
		    }

		    if (!gotElement)
		    {
		        throw new MalformedXInclusionResultError(Properties.Resources.MalformedXInclusionResult);
		    }

		    //That's all, folks
		    return false;
		} // Read()

	    /// <summary>
        /// Gets a value indicating whether the class can return line information.
        /// See <see cref="IXmlLineInfo.HasLineInfo"/>.
        /// </summary>        
        public bool HasLineInfo()
        {
            if (reader is IXmlLineInfo core)
            {
                return core.HasLineInfo();
            }
            return false;
        }

        /// <summary>
        /// Gets the current line number.
        /// See <see cref="IXmlLineInfo.LineNumber "/>.
        /// </summary>
        public int LineNumber
        {
            get
            {
                if (reader is IXmlLineInfo core)
                {
                    return core.LineNumber;
                }
                return 0;
            }
        }

        /// <summary>
        ///   	Gets the current line position.
        /// See <see cref="IXmlLineInfo.LinePosition "/>.
        /// </summary>
        public int LinePosition
        {
            get
            {
                if (reader is IXmlLineInfo core)
                {
                    return core.LinePosition;
                }
                return 0;
            }
        }

	    /// <summary>
		/// See <see cref="XmlTextReader.WhitespaceHandling"/>.
		/// </summary>
		public WhitespaceHandling WhitespaceHandling { get; set; }

	    /// <summary>
		/// XmlResolver to resolve external URI references
		/// </summary>
		public XmlResolver XmlResolver
		{
			set => xmlResolver = value;
		}

        /// <summary>
        /// Gets the encoding of the document.
        /// </summary>
        /// <remarks>If underlying XmlReader doesn't support Encoding property, null is returned.</remarks>
        public Encoding Encoding
        {
            get
            {
                if (reader is XmlTextReader xtr)
                {
                    return xtr.Encoding;
                }

                if (reader is XIncludingReader xir)
                {
                    return xir.Encoding;
                }
                return null;
            }
        }

		/// <summary>
		/// Flag indicating whether to emit <c>xml:base</c> as relative URI.
		/// True by default.
		/// </summary>
		public bool MakeRelativeBaseUri { get; set; } = true;

	    /// <summary>
		/// Flag indicating whether expose text inclusions
		/// as CDATA or as Text. By default it's Text.
		/// </summary>
		public bool ExposeTextInclusionsAsCDATA { get; set; }

	    //Dummy validation even handler
		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			//do nothing
		}

		/// <summary>
		/// Checks if given reader is positioned on a xi:include element.
		/// </summary>        
		private bool IsIncludeElement()
		{
		    return (
		               XIncludeKeywords.Equals(reader.NamespaceURI, keywords.XIncludeNamespace) ||
		               XIncludeKeywords.Equals(reader.NamespaceURI, keywords.OldXIncludeNamespace)
		           ) &&
		           XIncludeKeywords.Equals(reader.LocalName, keywords.Include);
		}

		/// <summary>
		/// /// Checks if given reader is positioned on a xi:fallback element.
		/// </summary>
		/// <returns></returns>
		private bool IsFallbackElement()
		{
		    return (
		               XIncludeKeywords.Equals(reader.NamespaceURI, keywords.XIncludeNamespace) ||
		               XIncludeKeywords.Equals(reader.NamespaceURI, keywords.OldXIncludeNamespace)
		           ) &&
		           XIncludeKeywords.Equals(reader.LocalName, keywords.Fallback);
		}

		/// <summary>
		/// Fetches resource by URI.
		/// </summary>        
		internal static Stream GetResource(string includeLocation,
			string accept, string acceptLanguage, out WebResponse response)
		{
			WebRequest wReq;
			try
			{
				wReq = WebRequest.Create(includeLocation);
			}
			catch (NotSupportedException nse)
			{
				throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.URISchemaNotSupported, includeLocation), nse);
			}
			catch (SecurityException se)
			{
				throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SecurityException, includeLocation), se);
			}
			//Add accept headers if this is HTTP request
		    if (wReq is HttpWebRequest httpReq)
			{
				if (accept != null)
				{
					TextUtils.CheckAcceptValue(accept);
					if (string.IsNullOrEmpty(httpReq.Accept))
					{
					    httpReq.Accept = accept;
					}
					else
					{
					    httpReq.Accept += "," + accept;
					}
				}
				if (acceptLanguage != null)
				{
					if (httpReq.Headers["Accept-Language"] == null)
					{
					    httpReq.Headers.Add("Accept-Language", acceptLanguage);
					}
					else
					{
					    httpReq.Headers["Accept-Language"] += "," + acceptLanguage;
					}
				}
			}
			try
			{
				response = wReq.GetResponse();
			}
			catch (WebException we)
			{
				throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.ResourceError, includeLocation), we);
			}
			return response.GetResponseStream();
		}

		/// <summary>
		/// Processes xi:include element.
		/// </summary>		
		private bool ProcessIncludeElement()
		{
			string href = reader.GetAttribute(keywords.Href);
			string xpointer = reader.GetAttribute(keywords.Xpointer);
			string parse = reader.GetAttribute(keywords.Parse);

			if (string.IsNullOrEmpty(href))
			{
				//Intra-document inclusion                                                
				if (parse == null || parse.Equals(keywords.Xml))
				{
					if (xpointer == null)
					{
						//Both href and xpointer attributes are absent in xml mode, 
						// => critical error
					    if (reader is IXmlLineInfo li && li.HasLineInfo())
						{
							throw new XIncludeSyntaxError(string.Format(
								CultureInfo.CurrentCulture,
								Properties.Resources.MissingHrefAndXpointerExceptionLong,
								reader.BaseURI,
								li.LineNumber, li.LinePosition));
						}
						else
						{
						    throw new XIncludeSyntaxError(string.Format(
						        CultureInfo.CurrentCulture,
						        Properties.Resources.MissingHrefAndXpointerException,
						        reader.BaseURI));
						}
					}
					//No support for intra-document refs                    
					throw new InvalidOperationException(Properties.Resources.IntradocumentReferencesNotSupported);
				}
				else if (parse.Equals(keywords.Text))
				{
					//No support for intra-document refs                    
					throw new InvalidOperationException(Properties.Resources.IntradocumentReferencesNotSupported);
				}
			}
			else
			{
				//Inter-document inclusion
				if (parse == null || parse.Equals(keywords.Xml))
				{
				    return ProcessInterDocXmlInclusion(href, xpointer);
				}
				else if (parse.Equals(keywords.Text))
				{
				    return ProcessInterDocTextInclusion(href);
				}
			}

			//Unknown "parse" attribute value, critical error
		    if (reader is IXmlLineInfo li2 && li2.HasLineInfo())
			{
				throw new XIncludeSyntaxError(string.Format(
					CultureInfo.CurrentCulture,
					Properties.Resources.UnknownParseAttrValueLong,
					parse,
					reader.BaseURI,
					li2.LineNumber, li2.LinePosition));
			}
			else
			{
			    throw new XIncludeSyntaxError(string.Format(
			        CultureInfo.CurrentCulture, Properties.Resources.UnknownParseAttrValue, parse));
			}
		}

		/// <summary>
		/// Resolves include location.
		/// </summary>
		/// <param name="href">href value</param>
		/// <returns>Include location.</returns>
		private Uri ResolveHref(string href)
		{
			Uri includeLocation;
			try
			{
			    Uri baseUri = reader.BaseURI == "" ? topBaseUri : new Uri(reader.BaseURI);
			    includeLocation = xmlResolver == null ? new Uri(baseUri, href) : xmlResolver.ResolveUri(baseUri, href);
			}
			catch (UriFormatException ufe)
			{
				throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidURI, href), ufe);
			}
			catch (Exception e)
			{
				throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.UnresolvableURI, href), e);
			}
			return includeLocation;
		}

		/// <summary>
		/// Skips content of an element using directly current reader's methods.
		/// </summary>
		private void SkipContent()
		{
			if (!reader.IsEmptyElement)
			{
				int depth = reader.Depth;
				while (reader.Read() && depth < reader.Depth)
				{
				}
			}
		}

		/// <summary>
		/// Fallback processing.
		/// </summary>
		/// <param name="depth"><c>xi:include</c> depth level.</param>    
		/// <param name="e">Resource error, which caused this processing.</param>
		/// <remarks>When inluding fails due to any resource error, <c>xi:inlcude</c> 
		/// element content is processed as follows: each child <c>xi:include</c> - 
		/// fatal error, more than one child <c>xi:fallback</c> - fatal error. No 
		/// <c>xi:fallback</c> - the resource error results in a fatal error.
		/// Content of first <c>xi:fallback</c> element is included in a usual way.</remarks>
		private bool ProcessFallback(int depth, Exception e)
		{
			//Read to the xi:include end tag
			while (reader.Read() && depth < reader.Depth)
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						if (IsIncludeElement())
						{
							//xi:include child of xi:include - fatal error
						    if (reader is IXmlLineInfo li && li.HasLineInfo())
							{
								throw new XIncludeSyntaxError(string.Format(
									CultureInfo.CurrentCulture,
									Properties.Resources.IncludeChildOfIncludeLong,
									BaseURI,
									li.LineNumber, li.LinePosition));
							}
							else
							{
							    throw new XIncludeSyntaxError(string.Format(
							        CultureInfo.CurrentCulture,
							        Properties.Resources.IncludeChildOfInclude,
							        BaseURI));
							}
						}
						if (IsFallbackElement())
						{
							//Found xi:fallback
							if (fallbackState.FallbackProcessed)
							{
							    if (reader is IXmlLineInfo li && li.HasLineInfo())
								{
									//Two xi:fallback                                 
									throw new XIncludeSyntaxError(string.Format(
										CultureInfo.CurrentCulture,
										Properties.Resources.TwoFallbacksLong,
										BaseURI,
										li.LineNumber, li.LinePosition));
								}
								else
								{
								    throw new XIncludeSyntaxError(string.Format(
								        CultureInfo.CurrentCulture, Properties.Resources.TwoFallbacks, BaseURI));
								}
							}
							if (reader.IsEmptyElement)
							{
								//Empty xi:fallback - nothing to include
								fallbackState.FallbackProcessed = true;
								break;
							}
							fallbackState.Fallbacking = true;
							fallbackState.FallbackDepth = reader.Depth;
							return Read();
						}
						else
							//Ignore anything else along with its content
						{
						    SkipContent();
						}

					    break;
				}
			}
			//xi:include content is read
			if (!fallbackState.FallbackProcessed)
				//No xi:fallback, fatal error
			{
			    throw new FatalResourceException(e);
			}
			else
			{
				//End of xi:include content processing, reset and go forth
				fallbackState = prevFallbackState;
				return Read();
			}
		}

		/// <summary>
		/// Skips xi:include element's content, while checking XInclude syntax (no 
		/// xi:include, no more than one xi:fallback).
		/// </summary>
		private void CheckAndSkipContent()
		{
			int depth = reader.Depth;
			bool fallbackElem = false;
			while (reader.Read() && depth < reader.Depth)
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						if (IsIncludeElement())
						{
							//xi:include child of xi:include - fatal error
						    if (reader is IXmlLineInfo li && li.HasLineInfo())
							{
								throw new XIncludeSyntaxError(string.Format(
									CultureInfo.CurrentCulture,
									Properties.Resources.IncludeChildOfIncludeLong,
									reader.BaseURI,
									li.LineNumber, li.LinePosition));
							}
							else
							{
							    throw new XIncludeSyntaxError(string.Format(
							        CultureInfo.CurrentCulture,
							        Properties.Resources.IncludeChildOfInclude,
							        reader.BaseURI));
							}
						}
						else if (IsFallbackElement())
						{
							//Found xi:fallback
							if (fallbackElem)
							{
								//More than one xi:fallback
							    if (reader is IXmlLineInfo li && li.HasLineInfo())
								{
									throw new XIncludeSyntaxError(string.Format(
										CultureInfo.CurrentCulture,
										Properties.Resources.TwoFallbacksLong,
										reader.BaseURI,
										li.LineNumber, li.LinePosition));
								}
								else
								{
								    throw new XIncludeSyntaxError(string.Format(
								        CultureInfo.CurrentCulture, Properties.Resources.TwoFallbacks, reader.BaseURI));
								}
							}
							else
							{
								fallbackElem = true;
								SkipContent();
							}
						}
						//Check anything else in XInclude namespace
						else if (XIncludeKeywords.Equals(reader.NamespaceURI, keywords.XIncludeNamespace))
						{
							throw new XIncludeSyntaxError(string.Format(
								CultureInfo.CurrentCulture, Properties.Resources.UnknownXIncludeElement, reader.Name));
						}
						else
							//Ignore everything else
						{
						    SkipContent();
						}

					    break;
				}
			}
		} // CheckAndSkipContent()

		/// <summary>
		/// Throws CircularInclusionException.
		/// </summary>        
		private void ThrowCircularInclusionError(XmlReader r, Uri url)
		{
		    if (r is IXmlLineInfo li && li.HasLineInfo())
			{
				throw new CircularInclusionException(url,
					BaseURI,
					li.LineNumber, li.LinePosition);
			}
			else
			{
			    throw new CircularInclusionException(url);
			}
		}

		/// <summary>
		/// Compares two languages as per IETF RFC 3066.
		/// </summary>        
		private bool AreDifferentLangs(string lang1, string lang2)
		{
			return lang1.ToLower() != lang2.ToLower();
		}

		/// <summary>
		/// Creates acquired infoset.
		/// </summary>        
		private string CreateAcquiredInfoset(Uri includeLocation)
		{
			if (cache == null)
			{
			    cache = new Dictionary<string, WeakReference>();
			}

		    if (cache.TryGetValue(includeLocation.AbsoluteUri, out WeakReference wr) && wr.IsAlive)
			{
				return (string)wr.Target;
			}

		    //Not cached or GCollected
		    Stream stream = GetResource(includeLocation.AbsoluteUri,
		        reader.GetAttribute(keywords.Accept),
		        reader.GetAttribute(keywords.AcceptLanguage), out WebResponse wRes);
		    var xir = new XIncludingReader(wRes.ResponseUri.AbsoluteUri, stream, nameTable)
		    {
		        WhitespaceHandling = WhitespaceHandling
		    };
		    var sw = new StringWriter();
		    var w = new XmlTextWriter(sw);
		    try
		    {
		        while (xir.Read())
		        {
		            w.WriteNode(xir, false);
		        }
		    }
		    finally
		    {
		        xir.Close();
		        w.Close();
		    }
		    string content = sw.ToString();
		    lock (cache)
		    {
		        if (!cache.ContainsKey(includeLocation.AbsoluteUri))
		        {
		            cache.Add(includeLocation.AbsoluteUri, new WeakReference(content));
		        }
		    }
		    return content;
		}

        /// <summary>
        /// Creates acquired infoset.
        /// </summary>
        /// <param name="sourceReader">Source reader</param>
        /// <param name="includeLocation">Base URI</param>
        private string CreateAcquiredInfoset(Uri includeLocation, TextReader sourceReader)
		{
			return CreateAcquiredInfoset(
				new XmlBaseAwareXmlReader(includeLocation.AbsoluteUri, sourceReader, nameTable));
		}

        /// <summary>
        /// Creates acquired infoset.
        /// </summary>
        /// <param name="sourceReader">Source reader</param>
        private string CreateAcquiredInfoset(XmlReader sourceReader)
		{
			//TODO: Try to stream out this stuff                                    
		    XIncludingReader xir = new XIncludingReader(sourceReader) {XmlResolver = xmlResolver};
		    StringWriter sw = new StringWriter();
			XmlTextWriter w = new XmlTextWriter(sw);
			try
			{
				while (xir.Read())
				{
				    w.WriteNode(xir, false);
				}
			}
			finally
			{
				xir.Close();
			    w.Close();
			}
			return sw.ToString();
		}

		/// <summary>
		/// Processes inter-document inclusion (xml mode).
		/// </summary>
		/// <param name="href">'href' attr value</param>
		/// <param name="xpointer">'xpointer attr value'</param>
		private bool ProcessInterDocXmlInclusion(string href, string xpointer)
		{
			//Include document as XML                                
			Uri includeLocation = ResolveHref(href);
			if (includeLocation.Fragment != string.Empty)
			{
			    throw new XIncludeSyntaxError(Properties.Resources.FragmentIDInHref);
			}

		    CheckLoops(includeLocation);
			if (xmlResolver == null)
			{
				//No custom resolver
				if (xpointer != null)
				{
					//Push current reader to the stack
					readers.Push(reader);
					//XPointers should be resolved against the acquired infoset, 
					//not the source infoset                                                                                          
					reader = new XPointerReader(includeLocation.AbsoluteUri,
						CreateAcquiredInfoset(includeLocation),
						xpointer);
				}
				else
				{
				    Stream stream = GetResource(includeLocation.AbsoluteUri,
						reader.GetAttribute(keywords.Accept),
						reader.GetAttribute(keywords.AcceptLanguage), out WebResponse wRes);
					//Push current reader to the stack
					readers.Push(reader);
				    //XmlReaderSettings settings = new XmlReaderSettings
				    //{
				    //    XmlResolver = xmlResolver,
				    //    IgnoreWhitespace = (WhitespaceHandling == WhitespaceHandling.None)
				    //};
				    XmlReader r = new XmlBaseAwareXmlReader(wRes.ResponseUri.AbsoluteUri, stream, nameTable);
					reader = r;
				}
				return Read();
			}
			else
			{
				//Custom resolver provided, let's ask him
				object resource;
				try
				{
					resource = xmlResolver.GetEntity(includeLocation, null, null);
				}
				catch (Exception e)
				{
					throw new ResourceException(Properties.Resources.CustomXmlResolverError, e);
				}
				if (resource == null)
				{
				    throw new ResourceException(Properties.Resources.CustomXmlResolverReturnedNull);
				}

			    //Push current reader to the stack
				readers.Push(reader);

				//Ok, we accept Stream, TextReader and XmlReader only                    
				if (resource is Stream)
				{
				    resource = new StreamReader((Stream)resource);
				}

			    if (xpointer != null)
				{
					if (resource is TextReader textReader)
					{
						//XPointers should be resolved against the acquired infoset, 
						//not the source infoset                                     
						reader = new XPointerReader(includeLocation.AbsoluteUri,
							CreateAcquiredInfoset(includeLocation, textReader),
							xpointer);
					}
					else if (resource is XmlReader)
					{
						XmlReader r = (XmlReader)resource;
						reader = new XPointerReader(r.BaseURI,
							CreateAcquiredInfoset(r), xpointer);
					}
					else
					{
						//Unsupported type
						throw new ResourceException(string.Format(
							CultureInfo.CurrentCulture,
							Properties.Resources.CustomXmlResolverReturnedUnsupportedType,
							resource.GetType().ToString()));
					}
				}
				else
				{
					//No XPointer   
					if (resource is TextReader textReader)
					{
					    reader = new XmlBaseAwareXmlReader(includeLocation.AbsoluteUri, textReader, nameTable);
					}
					else if (resource is XmlReader)
					{
					    reader = (XmlReader)resource;
					}
					else
					{
						//Unsupported type
						throw new ResourceException(string.Format(
							CultureInfo.CurrentCulture,
							Properties.Resources.CustomXmlResolverReturnedUnsupportedType,
							resource.GetType().ToString()));
					}
				}

				return Read();
			}
		}

		/// <summary>
		/// Process inter-document inclusion as text.
		/// </summary>
		/// <param name="href">'href' attr value</param>        
		private bool ProcessInterDocTextInclusion(string href)
		{
			//Include document as text                            
			string encoding = GetAttribute(keywords.Encoding);
			Uri includeLocation = ResolveHref(href);
			//No need to check loops when including as text
			//Push current reader to the stack
			readers.Push(reader);
			reader = new TextIncludingReader(includeLocation, encoding,
				reader.GetAttribute(keywords.Accept),
				reader.GetAttribute(keywords.AcceptLanguage),
				ExposeTextInclusionsAsCDATA);
			return Read();
		}


		/// <summary>
		/// Checks for inclusion loops.
		/// </summary>        
		private void CheckLoops(Uri url)
		{
			//Check circular inclusion  
			Uri baseUri = reader.BaseURI == "" ? topBaseUri : new Uri(reader.BaseURI);
			if (baseUri.Equals(url))
			{
			    ThrowCircularInclusionError(reader, url);
			}

		    foreach (XmlReader r in readers)
			{
				baseUri = r.BaseURI == "" ? topBaseUri : new Uri(r.BaseURI);
				if (baseUri.Equals(url))
				{
				    ThrowCircularInclusionError(reader, url);
				}
			}
		}

        private string GetBaseUri()
        {
            if (reader.BaseURI == string.Empty)
            {
                return string.Empty;
            }
            if (MakeRelativeBaseUri)
            {
                Uri baseUri = new Uri(reader.BaseURI);
                return topBaseUri.MakeRelativeUri(baseUri).ToString();
            }
            else
            {
                return reader.BaseURI;
            }
        }
	}
}
