using System.Xml;
using System.Text;
using System.IO;
using Mvp.Xml.Common.Xsl;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Exslt
{

	/// <summary>
	/// XSLT output method enumeration, see W3C XSLT 1.0 Recommendation at 
	/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>.    
	/// </summary>
	/// <remarks>Only <c>xml</c> and <c>text</c> methods are supported by this version of 
	/// the <c>MultiXmlTextWriter</c>.</remarks>
	internal enum OutputMethod { Xml, Text };

	/// <summary>
	/// This class represents redirected output state and properties.
	/// </summary>    
	internal class OutputState
	{
	    //private string storedDir;

	    /// <summary>
		/// Creates new <c>OutputState</c> with default properties values:
		/// UTF8 encoding, no indentation, nonstandalone document, XML output
		/// method.
		/// </summary>                                    
		public OutputState()
		{
			Encoding = Encoding.UTF8;
			Indent = false;
			Standalone = false;
			OmitXmlDeclaration = false;
			Method = OutputMethod.Xml;
		}

		/// <summary>
		/// Initializes the writer to write redirected output. 
		/// </summary>              
		/// <remarks>Depending on the <c>method</c> attribute value, 
		/// <c>XmlTextWriter</c> or <c>StreamWriter</c> is created. 
		/// <c>XmlTextWriter</c> is used for outputting XML and  
		/// <c>StreamWriter</c> - for plain text.
		/// </remarks>
		public void InitWriter(XmlResolver outResolver)
		{
			if (outResolver == null)
			{
				outResolver = new OutputResolver(Directory.GetCurrentDirectory());
			}

			// Save current directory
			//storedDir = Directory.GetCurrentDirectory();
			string outFile = outResolver.ResolveUri(null, Href).LocalPath;
			DirectoryInfo dir = Directory.GetParent(outFile);
			if (!dir.Exists)
			{
			    dir.Create();
			}

		    // Create writer
			if (Method == OutputMethod.Xml)
			{
				XmlWriter = new XmlTextWriter(outFile, Encoding);
				if (Indent)
				{
				    XmlWriter.Formatting = Formatting.Indented;
				}

			    if (!OmitXmlDeclaration)
				{
					if (Standalone)
					{
					    XmlWriter.WriteStartDocument(true);
					}
					else
					{
					    XmlWriter.WriteStartDocument();
					}
				}
			}
			else
			{
			    TextWriter = new StreamWriter(outFile, false, Encoding);
			}

		    // Set new current directory            
			//Directory.SetCurrentDirectory(dir.ToString());                                    
            Href = ""; // clean the href for the next usage
		}

		/// <summary>
		/// Closes the writer that was used to write redirected output.
		/// </summary>
		public void CloseWriter()
		{
			if (Method == OutputMethod.Xml)
			{
				if (!OmitXmlDeclaration)
				{
					XmlWriter.WriteEndDocument();
				}
				XmlWriter.Close();
			}
			else
			{
			    TextWriter.Close();
			}

		    // Restore previous current directory
			//Directory.SetCurrentDirectory(storedDir);
		}

		/// <summary>
		/// Specifies whether the result document should be written with 
		/// a standalone XML document declaration.
		/// </summary>
		/// <value>Standalone XML declaration as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>
		/// <remarks>The property does not affect output while output method is <c>text</c>.</remarks>            
		public bool Standalone { get; set; }

	    /// <summary>
		/// Specifies output method.
		/// </summary>
		/// <value>Output Method as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>        
		public OutputMethod Method { get; set; }

	    /// <summary>
		/// Specifies the URI where the result document should be written to.
		/// </summary>                            
		/// <value>Absolute or relative URI of the output document.</value>
		public string Href { get; set; }

	    /// <summary>
		/// Specifies the preferred character encoding of the result document.
		/// </summary>
		/// <value>Output encoding as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>
		public Encoding Encoding { get; set; }

	    /// <summary>
		/// Specifies whether the result document should be written in the 
		/// indented form.
		/// </summary>
		/// <value>Output document formatting as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>
		/// <remarks>The property does not affect output while output method is <c>text</c>.</remarks>
		public bool Indent { get; set; }

	    /// <summary>
		/// Specifies the public identifier to be used in the document 
		/// type declaration.
		/// </summary>
		/// <value>Public part of the output document type definition as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>
		/// <remarks>The property does not affect output while output method is <c>text</c>.</remarks>
		public string PublicDoctype { get; set; }

	    /// <summary>
		/// Specifies the system identifier to be used in the document 
		/// type declaration.
		/// </summary>
		/// <value>System part of the output document type definition as per W3C XSLT 1.0 Recommendation (see 
		/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>
		/// for more info).</value>
		/// <remarks>The property does not affect output while output method is <c>text</c>.</remarks>
		public string SystemDoctype { get; set; }

	    /// <summary>
		/// Actual <c>XmlTextWriter</c> used to write the redirected 
		/// result document.
		/// </summary>              
		/// <value><c>XmlWriter</c>, which is used to write the output document in XML method.</value>                      
		public XmlTextWriter XmlWriter { get; private set; }

	    /// <summary>
		/// Actual <c>TextWriter</c> used to write the redirected 
		/// result document in text output method.
		/// </summary>                                    
		/// <value><c>StreamWriter</c>, which is used to write the output document in text method.</value>
		public StreamWriter TextWriter { get; private set; }

	    /// <summary>
		/// Tree depth (used to detect end tag of the <c>exsl:document</c>).
		/// </summary>
		/// <value>Current output tree depth.</value>
		public int Depth { get; set; }

	    /// <summary>
		/// Specifies whether the XSLT processor should output an XML declaration.        
		/// </summary>        
		public bool OmitXmlDeclaration { get; set; }
	}
}
