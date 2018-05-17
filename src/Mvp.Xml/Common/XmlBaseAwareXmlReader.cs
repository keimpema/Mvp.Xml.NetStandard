using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Mvp.Xml.Common
{    
    /// <summary>
    /// Custom <see cref="XmlReader"/> supporting <a href="http://www.w3.org/TR/xmlbase/">XML Base</a>.
    /// </summary>
    /// <remarks>
    /// <para>Author: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>
    public class XmlBaseAwareXmlReader : XmlWrappingReader
    {
        private XmlBaseState state = new XmlBaseState();
        private Stack<XmlBaseState> states;

        private static XmlReaderSettings CreateReaderSettings() {
            var settings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
            //settings.ProhibitDtd = false;
            return settings;
        }

        private static XmlReaderSettings CreateReaderSettings(XmlResolver resolver)
        {
            XmlReaderSettings settings = CreateReaderSettings();            
            settings.XmlResolver = resolver;            
            return settings;
        } 
       
        private static XmlReaderSettings CreateReaderSettings(XmlNameTable nt)
        {
            XmlReaderSettings settings = CreateReaderSettings();            
            settings.NameTable = nt;
            return settings;
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given URI.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri)
            : base(Create(uri, CreateReaderSettings()))
        {
            state.BaseUri = new Uri(base.BaseURI);
            
        }

		/// <summary>
		/// Creates XmlBaseAwareXmlReader instance for given URI using the given resolver.
		/// </summary>        
		public XmlBaseAwareXmlReader(string uri, XmlResolver resolver)
            : base(Create(uri, CreateReaderSettings(resolver)))
        {            
            state.BaseUri = new Uri(base.BaseURI);         
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given URI and 
        /// name table.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, XmlNameTable nt)
            : base(Create(uri, CreateReaderSettings(nt)))
        {
            state.BaseUri = new Uri(base.BaseURI);
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given TextReader.
        /// </summary>        
        public XmlBaseAwareXmlReader(TextReader reader)
            : base(Create(reader, CreateReaderSettings())) 
        {
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given uri and 
        /// TextReader.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, TextReader reader)
            : base(Create(reader, CreateReaderSettings(), uri))
        {
            state.BaseUri = new Uri(base.BaseURI);
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given TextReader 
        /// and name table.
        /// </summary>        
        public XmlBaseAwareXmlReader(TextReader reader, XmlNameTable nt)
            : base(Create(reader, CreateReaderSettings(nt))) 
        {
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given uri, name table
        /// and TextReader.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, TextReader reader, XmlNameTable nt)
            : base(Create(reader, CreateReaderSettings(nt), uri))
        {
            state.BaseUri = new Uri(base.BaseURI);
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given stream.
        /// </summary>        
        public XmlBaseAwareXmlReader(Stream stream)
            : base(Create(stream, CreateReaderSettings()))
        {
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given uri and stream.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, Stream stream)
            : base(Create(stream, CreateReaderSettings(), uri))
        {
            state.BaseUri = new Uri(base.BaseURI);
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given uri and stream.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, Stream stream, XmlResolver resolver)
            : base(Create(stream, CreateReaderSettings(resolver), uri))
        {
            state.BaseUri = new Uri(base.BaseURI);            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given stream 
        /// and name table.
        /// </summary>        
        public XmlBaseAwareXmlReader(Stream stream, XmlNameTable nt)
            : base(Create(stream, CreateReaderSettings(nt))) 
        {
            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given stream,
        /// uri and name table.
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, Stream stream, XmlNameTable nt)
            : base(Create(stream, CreateReaderSettings(nt), uri))
        {
            state.BaseUri = new Uri(base.BaseURI);            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given uri and <see cref="XmlReaderSettings"/>.        
        /// </summary>        
        public XmlBaseAwareXmlReader(string uri, XmlReaderSettings settings)
            : base(Create(uri, settings))
        {            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given <see cref="TextReader"/> and <see cref="XmlReaderSettings"/>.        
        /// </summary>        
        public XmlBaseAwareXmlReader(TextReader reader, XmlReaderSettings settings)
            : base(Create(reader, settings))
        {            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given <see cref="Stream"/> and <see cref="XmlReaderSettings"/>.        
        /// </summary>        
        public XmlBaseAwareXmlReader(Stream stream, XmlReaderSettings settings)
            : base(Create(stream, settings))
        {            
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given <see cref="XmlReader"/> and <see cref="XmlReaderSettings"/>.        
        /// </summary>        
        public XmlBaseAwareXmlReader(XmlReader reader, XmlReaderSettings settings)
            : base(Create(reader, settings))
        {
        }        

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given 
        /// <see cref="TextReader"/>, <see cref="XmlReaderSettings"/>
        /// and base uri.
        /// </summary>        
        public XmlBaseAwareXmlReader(TextReader reader, XmlReaderSettings settings, string baseUri)
            : base(Create(reader, settings, baseUri))
        {
        }

        /// <summary>
        /// Creates XmlBaseAwareXmlReader instance for given 
        /// <see cref="Stream"/>, <see cref="XmlReaderSettings"/>
        /// and base uri.
        /// </summary>        
        public XmlBaseAwareXmlReader(Stream stream, XmlReaderSettings settings, string baseUri)
            : base(Create(stream, settings, baseUri))
        {
        }

        /// <summary>
        /// See <see cref="XmlTextReader.BaseURI"/>.
        /// </summary>
        public override string BaseURI => state.BaseUri == null ? "" : state.BaseUri.AbsoluteUri;

        /// <summary>
        /// See <see cref="XmlTextReader.Read"/>.
        /// </summary>
        public override bool Read()
        {
            bool baseRead = base.Read();
            if (baseRead)
            {
                if (NodeType == XmlNodeType.Element && HasAttributes)
                {
                    string baseAttr = GetAttribute("xml:base");
                    if (baseAttr == null)
                    {
                        return true;
                    }

                    Uri newBaseUri = state.BaseUri == null ? new Uri(baseAttr) : new Uri(state.BaseUri, baseAttr);

                    if (states == null)
                    {
                        states = new Stack<XmlBaseState>();
                    }

                    //Push current state and allocate new one
                    states.Push(state);
                    state = new XmlBaseState(newBaseUri, Depth);
                }
                else if (NodeType == XmlNodeType.EndElement)
                {
                    if (Depth == state.Depth && states != null && states.Count > 0)
                    {
                        //Pop previous state
                        state = states.Pop();
                    }
                }
            }
            return baseRead;
        }
    }

    internal class XmlBaseState
    {
        public XmlBaseState() { }

        public XmlBaseState(Uri baseUri, int depth)
        {
            BaseUri = baseUri;
            Depth = depth;
        }

        public Uri BaseUri { get; set; }
        public int Depth { get; set; }
    }
}
