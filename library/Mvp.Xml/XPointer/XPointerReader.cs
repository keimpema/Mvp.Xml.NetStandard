using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Collections.Generic;

using Mvp.Xml.Common.XPath;
using Mvp.Xml.Common;

namespace Mvp.Xml.XPointer
{
    /// <summary>
    /// <c>XPointerReader</c> is an <see cref="XmlReader"/> that implements XPointer Framework in
    /// a fast, non-caching, forward-only way. <c>XPointerReader</c> 
    /// supports XPointer Framework, element(), xmlns(), xpath1() and
    /// xpointer() (XPath subset only) XPointer schemes.
    /// </summary>
    /// <remarks><para>See <a href="http://www.codeplex.com/MVPXML/Wiki/View.aspx?title=XPointer.NET">XPointer.NET homepage</a> for more info.</para>
    /// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>
    public class XPointerReader : XmlReader, IHasXPathNavigator, IXmlLineInfo
    {
        //Underlying reader
        private XmlReader reader;
        //Nodes selected by xpointer
        private XPathNodeIterator pointedNodes;
        //Document cache
        private static IDictionary<string, WeakReference> cache;

        /// <summary>
        /// Initializes the <c>XPointerReader</c>.
        /// </summary>
        private void Init(XPathNavigator nav, string xpointer)
        {
            Pointer pointer = XPointerParser.ParseXPointer(xpointer);
            pointedNodes = pointer.Evaluate(nav);
            //There is always at least one identified node
            //XPathNodeIterator is already at the first node
            reader = new SubtreeXPathNavigator(pointedNodes.Current).ReadSubtree();
        }

        private XPathDocument CreateAndCacheDocument(XmlReader r)
        {
            string uri = r.BaseURI;
            XPathDocument doc = new XPathDocument(r, XmlSpace.Preserve);
            r.Close();

            //Can't cache documents with empty base URI
            if (!string.IsNullOrEmpty(uri))
            {
                lock (cache)
                {
                    if (!cache.ContainsKey(uri))
                    {
                        cache.Add(uri, new WeakReference(doc));
                    }
                }
            }
            return doc;
        }

        /// <summary>
        /// Creates <c>XPointerReader</c> instnace with given <see cref="IXPathNavigable"/>
        /// and xpointer.
        /// </summary>        
        public XPointerReader(IXPathNavigable doc, string xpointer)
            : this(doc.CreateNavigator(), xpointer) { }

        /// <summary>
        /// Creates <c>XPointerReader</c> instnace with given <see cref="XPathNavigator"/>
        /// and xpointer.
        /// </summary>        
        public XPointerReader(XPathNavigator nav, string xpointer)
        {
            Init(nav, xpointer);
        }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, string xpointer)
            : this(new XmlBaseAwareXmlReader(uri), xpointer) { }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, nametable and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, XmlNameTable nt, string xpointer)
            : this(new XmlBaseAwareXmlReader(uri, nt), xpointer) { }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream, nametable and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, XmlNameTable nt, string xpointer)
            : this(new XmlBaseAwareXmlReader(uri, stream, nt), xpointer) { }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given uri, stream and xpointer.
        /// </summary>	    
        public XPointerReader(string uri, Stream stream, string xpointer)
            : this(uri, stream, new NameTable(), xpointer) { }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given XmlReader and xpointer.
        /// Additionally sets a flag whether to support schema-determined IDs.
        /// </summary>	    
        public XPointerReader(XmlReader reader, string xpointer)
        {
            XPathDocument doc;
            if (cache == null)
            {
                cache = new Dictionary<string, WeakReference>();
            }

            if (!string.IsNullOrEmpty(reader.BaseURI) &&
                cache.TryGetValue(reader.BaseURI, out WeakReference wr) &&
                wr.IsAlive)
            {
                doc = (XPathDocument)wr.Target;
                reader.Close();
            }
            else
            {
                //Not cached or GCollected or no base Uri                
                doc = CreateAndCacheDocument(reader);
            }
            Init(doc.CreateNavigator(), xpointer);
        }

        /// <summary>
        /// Creates <c>XPointerReader</c> instance with given
        /// document's URI and content.
        /// </summary>
        /// <param name="uri">XML document's base URI</param>
        /// <param name="content">XML document's content</param>
        /// <param name="xpointer">XPointer pointer</param>        
        public XPointerReader(string uri, string content, string xpointer)
        {
            XPathDocument doc;
            if (cache == null)
            {
                cache = new Dictionary<string, WeakReference>();
            }

            if (cache.TryGetValue(uri, out WeakReference wr) && wr.IsAlive)
            {
                doc = (XPathDocument)wr.Target;
            }
            else
            {
                //Not cached or GCollected                        
                //XmlReader r = new XmlBaseAwareXmlReader(uri, new StringReader(content));
                XmlReaderSettings settings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
                //settings.ProhibitDtd = false;
                XmlReader r = Create(new StringReader(content), settings, uri);
                doc = CreateAndCacheDocument(r);
            }
            Init(doc.CreateNavigator(), xpointer);
        }

        /// <summary>See <see cref="XmlReader.AttributeCount"/>.</summary>
        public override int AttributeCount => reader.AttributeCount;

        /// <summary>See <see cref="XmlReader.BaseURI"/>.</summary>
        public override string BaseURI => reader.BaseURI;

        /// <summary>See <see cref="XmlReader.HasValue"/>.</summary>
        public override bool HasValue => reader.HasValue;

        /// <summary>See <see cref="XmlReader.IsDefault"/>.</summary>
        public override bool IsDefault => reader.IsDefault;

        /// <summary>See <see cref="XmlReader.Name"/>.</summary>
        public override string Name => reader.Name;

        /// <summary>See <see cref="XmlReader.LocalName"/>.</summary>
        public override string LocalName => reader.LocalName;

        /// <summary>See <see cref="XmlReader.NamespaceURI"/>.</summary>
        public override string NamespaceURI => reader.NamespaceURI;

        /// <summary>See <see cref="XmlReader.NameTable"/>.</summary>
        public override XmlNameTable NameTable => reader.NameTable;

        /// <summary>See <see cref="XmlReader.NodeType"/>.</summary>
        public override XmlNodeType NodeType => reader.NodeType;

        /// <summary>See <see cref="XmlReader.Prefix"/>.</summary>
        public override string Prefix => reader.Prefix;

        /// <summary>See <see cref="XmlReader.QuoteChar"/>.</summary>
        public override char QuoteChar => reader.QuoteChar;

        /// <summary>See <see cref="XmlReader.Close"/>.</summary>
        public override void Close()
        {
            reader?.Close();
        }

        /// <summary>See <see cref="XmlReader.Depth"/>.</summary>
        public override int Depth => reader.Depth;

        /// <summary>See <see cref="XmlReader.EOF"/>.</summary>
        public override bool EOF => reader.EOF;

        /// <summary>See <see cref="XmlReader.GetAttribute(int)"/>.</summary>
        public override string GetAttribute(int i)
        {
            return reader.GetAttribute(i);
        }

        /// <summary>See <see cref="XmlReader.GetAttribute(string)"/>.</summary>
        public override string GetAttribute(string name)
        {
            return reader.GetAttribute(name);
        }

        /// <summary>See <see cref="XmlReader.GetAttribute(string, string)"/>.</summary>
        public override string GetAttribute(string name, string namespaceUri)
        {
            return reader.GetAttribute(name, namespaceUri);
        }

        /// <summary>See <see cref="XmlReader.IsEmptyElement"/>.</summary>
        public override bool IsEmptyElement => reader.IsEmptyElement;

        /// <summary>See <see cref="XmlReader.LookupNamespace"/>.</summary>
        public override string LookupNamespace(string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        /// <summary>See <see cref="XmlReader.MoveToAttribute(int)"/>.</summary>
        public override void MoveToAttribute(int i)
        {
            reader.MoveToAttribute(i);
        }

        /// <summary>See <see cref="XmlReader.MoveToAttribute(string)"/>.</summary>
        public override bool MoveToAttribute(string name)
        {
            return reader.MoveToAttribute(name);
        }

        /// <summary>See <see cref="XmlReader.MoveToAttribute(string, string)"/>.</summary>
        public override bool MoveToAttribute(string name, string ns)
        {
            return reader.MoveToAttribute(name, ns);
        }

        /// <summary>See <see cref="XmlReader.MoveToElement"/>.</summary>
        public override bool MoveToElement()
        {
            return reader.MoveToElement();
        }

        /// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/>.</summary>
        public override bool MoveToFirstAttribute()
        {
            return reader.MoveToFirstAttribute();
        }

        /// <summary>See <see cref="XmlReader.MoveToNextAttribute"/>.</summary>
        public override bool MoveToNextAttribute()
        {
            return reader.MoveToNextAttribute();
        }

        /// <summary>See <see cref="XmlReader.ReadAttributeValue"/>.</summary>
        public override bool ReadAttributeValue()
        {
            return reader.ReadAttributeValue();
        }

        /// <summary>See <see cref="XmlReader.ReadState"/>.</summary>            
        public override ReadState ReadState => reader.ReadState;

        /// <summary>See <see cref="XmlReader.this[int]"/>.</summary>
        public override string this[int i] => reader[i];

        /// <summary>See <see cref="XmlReader.this[string]"/>.</summary>
        public override string this[string name] => reader[name];

        /// <summary>See <see cref="XmlReader.this[string, string]"/>.</summary>
        public override string this[string name, string namespaceUri] => reader[name, namespaceUri];

        /// <summary>See <see cref="XmlReader.ResolveEntity"/>.</summary>
        public override void ResolveEntity()
        {
            reader.ResolveEntity();
        }

        /// <summary>See <see cref="XmlReader.XmlLang"/>.</summary>
        public override string XmlLang => reader.XmlLang;

        /// <summary>See <see cref="XmlReader.XmlSpace"/>.</summary>
        public override XmlSpace XmlSpace => reader.XmlSpace;

        /// <summary>See <see cref="XmlReader.Value"/>.</summary>
        public override string Value => reader.Value;

        /// <summary>See <see cref="XmlReader.ReadInnerXml"/>.</summary>
        public override string ReadInnerXml()
        {
            return reader.ReadInnerXml();
        }

        /// <summary>See <see cref="XmlReader.ReadOuterXml"/>.</summary>
        public override string ReadOuterXml()
        {
            return reader.ReadOuterXml();
        }

        /// <summary>See <see cref="XmlReader.ReadString"/>.</summary>
        public override string ReadString()
        {
            return reader.ReadString();
        }

        /// <summary>See <see cref="XmlReader.Read"/>.</summary>
        public override bool Read()
        {
            bool baseRead = reader.Read();
            if (baseRead)
            {
                return true;
            }
            else if (pointedNodes != null)
            {
                if (pointedNodes.MoveNext())
                {
                    reader = new SubtreeXPathNavigator(pointedNodes.Current).ReadSubtree();
                    return reader.Read();
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the XPathNavigator for the current context or position.
        /// </summary>
        /// <returns></returns>
        public XPathNavigator GetNavigator()
        {
            return pointedNodes.Current.Clone();
        }

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
    }
}
