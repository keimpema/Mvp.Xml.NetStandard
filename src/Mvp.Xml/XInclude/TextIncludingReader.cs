using System;
using System.Xml;
using System.IO;
using System.Net;
using System.Text;
using System.Globalization;

namespace Mvp.Xml.XInclude
{
	/// <summary>
	/// Custom <c>XmlReader</c>, handler for parse="text" case.	
	/// </summary>
	/// <author>Oleg Tkachenko, http://www.xmllab.net</author>
	/// <remarks>
	/// Allows to read specified resource as a text node.
	/// </remarks>
	internal class TextIncludingReader : XmlReader
	{
	    private readonly string encoding;
		private ReadState state;
		private string value;
		private readonly Uri includeLocation;
	    private readonly string accept;
	    private readonly string acceptLanguage;
		private readonly string baseUri;
		private readonly bool exposeCdata;

	    public TextIncludingReader(Uri includeLocation, string encoding,
			string accept, string acceptLanguage, bool exposeCdata)
		{
			this.includeLocation = includeLocation;
			baseUri = includeLocation.AbsoluteUri;
			this.encoding = encoding;
			state = ReadState.Initial;
			this.accept = accept;
			this.acceptLanguage = acceptLanguage;
			this.exposeCdata = exposeCdata;
		}

		public TextIncludingReader(string value, bool exposeCdata)
		{
			state = ReadState.Initial;
			this.exposeCdata = exposeCdata;
			this.value = value;
		}

	    public override int AttributeCount => 0;

	    public override string BaseURI => baseUri;

	    public override int Depth => state == ReadState.Interactive ? 1 : 0;

	    public override bool EOF => state == ReadState.EndOfFile;

	    public override bool HasValue => state == ReadState.Interactive;

	    public override bool IsDefault => false;

	    public override bool IsEmptyElement => false;

	    public override string this[int index] => string.Empty;

	    public override string this[string qname] => string.Empty;

	    public override string this[string localname, string nsuri] => string.Empty;

	    public override string LocalName => string.Empty;

	    public override string Name => string.Empty;

	    public override string NamespaceURI => string.Empty;

	    public override XmlNameTable NameTable => null;

	    public override XmlNodeType NodeType => state == ReadState.Interactive ?
	        exposeCdata ? XmlNodeType.CDATA : XmlNodeType.Text
	        : XmlNodeType.None;

	    public override string Prefix => string.Empty;

	    public override char QuoteChar => '"';

	    public override ReadState ReadState => state;

	    public override string Value => state == ReadState.Interactive ? value : string.Empty;

	    public override string XmlLang => string.Empty;

	    public override XmlSpace XmlSpace => XmlSpace.None;

	    public override void Close()
		{
			state = ReadState.Closed;
		}

		public override string GetAttribute(int index)
		{
			throw new ArgumentOutOfRangeException("index", index, "No attributes exposed");
		}

		public override string GetAttribute(string qname)
		{
			return null;
		}

		public override string GetAttribute(string localname, string nsuri)
		{
			return null;
		}

		public override string LookupNamespace(string prefix)
		{
			return null;
		}

		public override void MoveToAttribute(int index) { }

		public override bool MoveToAttribute(string qname)
		{
			return false;
		}

		public override bool MoveToAttribute(string localname, string nsuri)
		{
			return false;
		}

		public override bool MoveToElement()
		{
			return false;
		}

		public override bool MoveToFirstAttribute()
		{
			return false;
		}

		public override bool MoveToNextAttribute()
		{
			return false;
		}

		public override bool ReadAttributeValue()
		{
			return false;
		}

		public override string ReadInnerXml()
		{
			return state == ReadState.Interactive ? value : string.Empty;
		}

		public override string ReadOuterXml()
		{
			return state == ReadState.Interactive ? value : string.Empty;
		}

		public override string ReadString()
		{
			return state == ReadState.Interactive ? value : string.Empty;
		}

		public override void ResolveEntity() { }

		public override bool Read()
		{
			switch (state)
			{
				case ReadState.Initial:
					if (value == null)
					{
					    Stream stream = XIncludingReader.GetResource(includeLocation.AbsoluteUri,
							accept, acceptLanguage, out WebResponse wRes);
					    /* According to the spec, encoding should be determined as follows:
							* external encoding information, if available, otherwise
							* if the media type of the resource is text/xml, application/xml, 
							  or matches the conventions text/*+xml or application/*+xml as 
							  described in XML Media Types [IETF RFC 3023], the encoding is 
							  recognized as specified in XML 1.0, otherwise
							* the value of the encoding attribute if one exists, otherwise  
							* UTF-8.
						*/
						try
						{
							//TODO: try to get "content-encoding" from wRes.Headers collection?
							//If mime type is xml-aware, get resource encoding as per XML 1.0
							string contentType = wRes.ContentType.ToLower();
						    StreamReader reader;
						    if (contentType == "text/xml" ||
								contentType == "application/xml" ||
								contentType.StartsWith("text/") && contentType.EndsWith("+xml") ||
								contentType.StartsWith("application/") && contentType.EndsWith("+xml"))
							{
								//Yes, that's xml, let's read encoding from the xml declaration                    
								reader = new StreamReader(stream, GetEncodingFromXmlDecl(baseUri));
							}
							else if (encoding != null)
							{
								//Try to use user-specified encoding
								Encoding enc;
								try
								{
									enc = Encoding.GetEncoding(encoding);
								}
								catch (Exception e)
								{
									throw new ResourceException(string.Format(
										CultureInfo.CurrentCulture,
										Properties.Resources.NotSupportedEncoding,
										encoding), e);
								}
								reader = new StreamReader(stream, enc);
							}
							else
								//Fallback to UTF-8
							{
							    reader = new StreamReader(stream, Encoding.UTF8);
							}

						    value = reader.ReadToEnd();
							TextUtils.CheckForNonXmlChars(value);
						}
						//catch (ResourceException re)
						//{
						//	throw re;
						//}
						catch (OutOfMemoryException oome)
						{
							//Crazy include - memory is out
							//TODO: what about reading by chunks?
							throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.OutOfMemoryWhileFetchingResource, baseUri), oome);
						}
						catch (IOException ioe)
						{
							throw new ResourceException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.IOErrorWhileFetchingResource, baseUri), ioe);
						}
					}
					state = ReadState.Interactive;
					return true;
				case ReadState.Interactive:
					//No more input
					state = ReadState.EndOfFile;
					return false;
				default:
					return false;
			}
		} // Read()

	    /// <summary>
		/// Reads encoding from the XML declarartion.
		/// </summary>
		/// <param name="href">URI reference indicating the location 
		/// of the resource to inlclude.</param>		
		/// <returns>The document encoding as per XML declaration.</returns>
		/// <exception cref="ResourceException">Resource error.</exception>
		private static Encoding GetEncodingFromXmlDecl(string href)
		{
		    var tmpReader = new XmlTextReader(href)
		    {
		        DtdProcessing = DtdProcessing.Parse,
		        WhitespaceHandling = WhitespaceHandling.None
		    };
		    //tmpReader.ProhibitDtd = false;
		    try
			{
				while (tmpReader.Read() && tmpReader.Encoding == null) { }
				Encoding enc = tmpReader.Encoding;
				return enc ?? Encoding.UTF8;
			}
			finally
			{
				tmpReader.Close();
			}
		}
	}
}
