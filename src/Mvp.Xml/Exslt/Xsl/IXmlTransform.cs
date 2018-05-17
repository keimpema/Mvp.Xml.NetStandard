using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Common.Xsl
{

	/// <summary>
	/// New experimental generic XML transform interface. Defines an API for
	/// transforming <see cref="XmlInput"/> into <see cref="XmlOutput"/>.
	/// </summary>
	public interface IXmlTransform
	{
		/// <summary>
		/// Transforms given <see cref="XmlInput"/> into <see cref="XmlOutput"/>.
		/// </summary>
		/// <param name="defaulDocument">Default input XML document</param>
		/// <param name="args">Parameters</param>
		/// <param name="output">Represents the transformation's output</param>
        /// <returns>Transformation output</returns>
        XmlOutput Transform(XmlInput defaulDocument, XsltArgumentList args, XmlOutput output);

		/// <summary>
		/// Defines default output settings.
		/// </summary>
		XmlWriterSettings OutputSettings { get; }
	}

    /// <summary>
    /// XmlInput class represents generic XML input to a trasnformation. The actual
    /// XML to be transformed can be provided as string URI, <see cref="Stream"/>,
    /// <see cref="TextReader"/>, <see cref="XmlReader"/> or <see cref="IXPathNavigable"/>.
    /// Optional <see cref="XmlResolver"/> is used to resolve external references when
    /// loading input XML document and URIs in a "document()" function calls during
    /// transformation.
    /// </summary>
    public class XmlInput
    {
        internal readonly object Source;
        internal readonly XmlResolver Resolver;

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an 
        /// <see cref="XmlReader"/>. Also registers an <see cref="XmlResolver"/> to be used
        /// for resolving external references in the XML document and document() function.
        /// </summary>
        /// <param name="reader">Input XML document</param>
        /// <param name="resolver"><see cref="XmlResolver"/> to resolve external references</param>
        public XmlInput(XmlReader reader, XmlResolver resolver)
        {
            Source = reader;
            Resolver = resolver;
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as a 
        /// <see cref="TextReader"/>. Also registers an <see cref="XmlResolver"/> to be used
        /// for resolving external references in the XML document and document() function.
        /// </summary>
        /// <param name="reader">Input XML document</param>
        /// <param name="resolver"><see cref="XmlResolver"/> to resolve external references</param>
        public XmlInput(TextReader reader, XmlResolver resolver)
        {
            Source = reader;
            Resolver = resolver;
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as a 
        /// <see cref="Stream"/>. Also registers an <see cref="XmlResolver"/> to be used
        /// for resolving external references in the XML document and document() function.
        /// </summary>
        /// <param name="stream">Input XML document</param>
        /// <param name="resolver"><see cref="XmlResolver"/> to resolve external references</param>
        public XmlInput(Stream stream, XmlResolver resolver)
        {
            Source = stream;
            Resolver = resolver;
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an URI. 
        /// Also registers an <see cref="XmlResolver"/> to be used
        /// for resolving external references in the XML document and document() function.
        /// </summary>
        /// <param name="uri">Input XML document</param>
        /// <param name="resolver"><see cref="XmlResolver"/> to resolve external references</param>
        public XmlInput(string uri, XmlResolver resolver)
        {
            Source = uri;
            Resolver = resolver;
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an 
        /// <see cref="IXPathNavigable"/>. Also registers an <see cref="XmlResolver"/> to be used
        /// for resolving document() function.
        /// </summary>
        /// <param name="nav">Input XML document</param>
        /// <param name="resolver"><see cref="XmlResolver"/> to resolve external references</param>
        public XmlInput(IXPathNavigable nav, XmlResolver resolver)
        {
            Source = nav;
            Resolver = resolver;
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an 
        /// <see cref="XmlReader"/>. 
        /// </summary>
        /// <param name="reader">Input XML document</param>        
        public XmlInput(XmlReader reader) : this(reader, new DefaultXmlResolver())
        {
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as a 
        /// <see cref="TextReader"/>. 
        /// </summary>
        /// <param name="reader">Input XML document</param>        
        public XmlInput(TextReader reader) : this(reader, new DefaultXmlResolver())
        {
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as a 
        /// <see cref="Stream"/>. 
        /// </summary>
        /// <param name="stream">Input XML document</param>        
        public XmlInput(Stream stream) : this(stream, new DefaultXmlResolver())
        {
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an URI.        
        /// </summary>
        /// <param name="uri">Input XML document</param>        
        public XmlInput(string uri) : this(uri, new DefaultXmlResolver())
        {
        }

        /// <summary>
        /// Creates new XmlInput object for an XML document provided as an 
        /// <see cref="IXPathNavigable"/>. 
        /// </summary>
        /// <param name="nav">Input XML document</param>        
        public XmlInput(IXPathNavigable nav) : this(nav, new DefaultXmlResolver())
        {
        }

        // We can add set of implicit constructors. 
        // I am not shre that this will be for good, so I commented them for now.
        //public static implicit operator XmlInput(XmlReader      reader) { return new XmlInput(reader); }
        //public static implicit operator XmlInput(TextReader     reader) { return new XmlInput(reader); }
        //public static implicit operator XmlInput(Stream         stream) { return new XmlInput(stream); }
        //public static implicit operator XmlInput(String         uri   ) { return new XmlInput(uri   ); }
        //public static implicit operator XmlInput(XPathNavigator nav   ) { return new XmlInput(nav   ); } // the trick doesn't work with interfaces
    }

    /// <summary>
	/// Resolves URIs from the current working directory.
	/// </summary>
	public class OutputResolver : XmlUrlResolver
	{
		private readonly Uri baseUri;

		/// <summary>
		/// Initializes a new instance of the <see cref="OutputResolver"/> class, 
		/// with the given <paramref name="baseUri"/> appended to the current directory.
		/// </summary>
		/// <param name="baseUri">The Uri to append to the current directory.</param>
		public OutputResolver(string baseUri)
		{
            if (string.IsNullOrEmpty(baseUri))
            {
                baseUri = ".";
            }
			this.baseUri = new Uri(new Uri(Directory.GetCurrentDirectory() + "/"), baseUri + "/");
		}

		/// <summary>
		/// Resolves the Uri using the <c>baseUri</c> determined in the constructor.
		/// </summary>
		/// <param name="_">Not used.</param>
		/// <param name="relativeUri">The relative Uri to resolve</param>
		public override Uri ResolveUri(Uri _, string relativeUri)
		{
			return base.ResolveUri(baseUri, relativeUri);
		}
	}

    /// <summary>
    /// XmlOutput class represents generic XML transformation output. An output XML
    /// can be written to an URI, <see cref="Stream"/>, <see cref="TextWriter"/> or
    /// <see cref="XmlWriter"/>.
    /// </summary>
    public class XmlOutput
    {
        internal readonly object Destination;

        /// <summary>
        /// Resolver to use to acquire external resources.
        /// </summary>
        public XmlResolver XmlResolver { get; set; }

        /// <summary>
        /// Creates new XmlOutput object over an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write output to</param>
        public XmlOutput(XmlWriter writer)
        {
            Destination = writer;
        }

        /// <summary>
        /// Creates new XmlOutput object over a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write output to</param>
        public XmlOutput(TextWriter writer)
        {
            Destination = writer;
        }

        /// <summary>
        /// Creates new XmlOutput object over a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to write output to</param>
        public XmlOutput(Stream stream)
        {
            Destination = stream;
        }

        /// <summary>
        /// Creates new XmlOutput object over an URI.
        /// </summary>
        /// <param name="uri">An URI to write output to</param>
        public XmlOutput(string uri)
        {
            Destination = uri;
        }

        // We will add overrides with XmlOutputResolver here later to support multiple output documents (<xsl:result-document>)
    }

    /// <summary>
	/// XmlUrlResolver wrapper allowing us to recognize the case when no
	/// XmlResolver was passed.
	/// </summary>
	internal class DefaultXmlResolver : XmlUrlResolver
	{
	}
}
