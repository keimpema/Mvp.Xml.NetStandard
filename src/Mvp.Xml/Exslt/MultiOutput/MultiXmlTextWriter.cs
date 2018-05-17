using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.IO;
using System.Security;
using Mvp.Xml.Common.Xsl;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Exslt
{
	/// <summary>
	/// Specifies the redirecting state of the <c>MultiXmlTextWriter</c>.
	/// </summary>
	internal enum RedirectState
	{
		/// <summary>
		/// The output is being relayed further (default).
		/// </summary>
		Relaying,
		/// <summary>
		/// The output is being redirected.
		/// </summary>
		Redirecting,
		/// <summary>
		/// <c>&lt;exsl:document></c> attributes are being written
		/// </summary>	        
		WritingRedirectElementAttrs,
		/// <summary>
		/// <c>&lt;exsl:document></c> attribute value is being written
		/// </summary>
		WritingRedirectElementAttrValue
	}

	/// <summary>
	/// <para><c>MultiXmlTextWriter</c> class extends standard <see cref="XmlTextWriter"/> class 
	/// and represents an XML writer that provides a fast, 
	/// non-cached, forward-only way of generating multiple output files containing
	/// either text data or XML data that conforms to the W3C Extensible Markup 
	/// Language (XML) 1.0 and the Namespaces in XML recommendations.</para>	
	/// </summary>
	/// <remarks>
	/// <para>Instances of <c>MultiXmlTextWriter</c> class regognize special element 
	/// (<c>&lt;exsl:document></c> in <c>"http://exslt.org/common"</c> namespace) as
	/// instruction to redirect the output of this element's content to another file. 
	/// When using with <c>XslTransform</c> class, <c>MultiXmlTextWriter</c> class 
	/// allows to generate multiple XSL Transfromation results.</para>	
	/// <para><c>MultiXmlTextWriter</c> class extends <c>System.Xml.XmlTextWriter</c>
	/// class, therefore its instances can be passed directly to the overloaded 
	/// <c>XslTransform.Transform()</c> method, which accepts <c>XmlWriter</c> as 
	/// object to write the transformation result to. All actual XML writing work 
	/// <c>MultiXmlTextWriter</c> class delegates to its base class, but it overrides 
	/// several <c>XmlTextWriter</c> class methods to implement output switching logic 
	/// as follows: once <c>&lt;exsl:document></c> element start tag is detected in 
	/// the XML stream, new writer (<c>XmlTextWriter</c> or <c>StreamWriter</c> depending on
	/// <c>method</c> attribute value) object is created with parameters as specified 
	/// in the attributes of the <c>&lt;exsl:document></c> element and the output 
	/// is switched to this newly created writer untill the end tag of the 
	/// <c>&lt;exsl:document></c> element is encountered.</para>
	/// <para><c>&lt;exsl:document></c> element syntax is as follows:
	/// <code>
	/// &lt;exsl:document
	///    href = { uri-reference }
	///    method = { "xml" | "text" }
	///    encoding = { string }
	///    omit-xml-declaration = { "yes" | "no" }
	///    standalone = { "yes" | "no" }
	///    doctype-public = { string }
	///    doctype-system = { string }
	///    indent = { "yes" | "no" }
	///    &lt;-- Content: template -->
	/// &lt;/exsl:document>
	/// </code></para>
	/// <para>
	/// The <c>href</c> attribute specifies where new result document should be stored, 
	/// it must be an absolute or relative URI. Relative URIs are resolved 
	/// relatively to the parent result document base URI. If the <c>href</c> attribute
	/// specifies that the output should be redirected to a file in a directory
	/// and that directory does not exist, it will be created. This allows to create 
	/// directory trees of any complexity.</para>
	/// <para>Semantics of the rest attributes is as defined in W3C XSLT 1.0 Recommendation 
	/// for <c>&lt;xsl:output></c> element, see 
	/// <a href="http://www.w3.org/TR/xslt.html#output">http://www.w3.org/TR/xslt.html#output</a>.
	/// </para>
	/// <para><b>Note:</b> <c>&lt;exsl:document></c> element namespace prefix must be bound to 
	/// <c>"http://exslt.org/common"</c> namespace URI in order to be recognized by 
	/// <c>MultiXmlTextWriter</c> class as a redirecting instruction. </para>
	/// </remarks>
	/// <example>This example shows how to use <c>MultiXmlTextWriter</c> along with 
	/// <c>XslTransform</c> to achieve create multiple result documents in one
	/// transfromation run:
	/// <para>In XSL stylesheet document declare <c>"http://exslt.org/common"</c> namespace
	/// and whenever you want to create new result document make use of <c>&lt;exsl:documnet></c>
	/// element:<br/>
	/// <c>style.xsl</c> stylesheet fragment:
	/// <code>
	/// <![CDATA[
	/// <xsl:stylesheet version="1.0" 
	///   xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	///   ]]><b>xmlns:exsl= "http://exslt.org/common"</b><![CDATA[ 
	///   ]]><b>exclude-result-prefixes="exsl"</b>><![CDATA[ 
	///   <xsl:template match="book">
	///     <!-- Builds frameset -->
	///     <html> 
	///       <head>
	///         <title><xsl:value-of select="@title"/></title> 
	///       </head>
	///       <frameset cols="20%,80%"> 
	///         <frame src="toc.html"/> 
	///         <!-- Builds table of contents output document -->    
	///         ]]><b>&lt;exsl:document href="toc.html" indent="yes"></b><![CDATA[
	///           <html>
	///             <head>
	///               <title>Table of Contents</title>
	///             </head>
	///             <body>
	///               <ul>
	///                 <xsl:apply-templates mode="toc"/> 
	///               </ul> 
	///             </body>                      
	///           </html>
	///         ]]><b>&lt;/exsl:document></b><![CDATA[ 
	///         <frame src="{chapter[1]/@id}.html" name="body" />
	///         <xsl:apply-templates />
	///       </frameset> 
	///     </html>
	///     ...  
	///
	/// ]]>
	/// </code>
	/// C# code fragment:	
	/// <code>
	/// XPathDocument doc = new XPathDocument("book.xml");
	/// XslTransform xslt = new XslTransform();
	/// xslt.Load("style.xsl");	            
	/// MultiXmlTextWriter multiWriter = 
	///     new MultiXmlTextWriter("index.html", Encoding.UTF8);
	/// multiWriter.Formatting = Formatting.Indented;
	/// xslt.Transform(doc, null, multiWriter);
	/// </code>
	/// </para>
	/// </example>
	public class MultiXmlTextWriter : XmlTextWriter
	{
		/// <summary>
		/// This constant is the namespace <c>&lt;exsl:document></c> element 
		/// should belong to in order to be recognized as redirect instruction.
		/// It's <c>"http://exslt.org/common"</c> as defined by 
		/// <a href="http://www.exslt.org">EXSLT community initiative</a>.
		/// </summary>	    	     
		protected const string RedirectNamespace = "http://exslt.org/common";

		/// <summary>
		/// This constant is the redirect instruction element name.
		/// It's <c>"document"</c> as defined by 
		/// <a href="http://www.exslt.org">EXSLT community initiative</a>.
		/// </summary>
		protected const string RedirectElementName = "document";

	    private readonly XmlResolver outResolver;

		// Stack of output states
	    private Stack states;
		// Current output state
	    private OutputState state;
		// Currently processed attribute name 
	    private string currentAttributeName;

		//Redirecting state - relaying by default
	    private RedirectState redirectState = RedirectState.Relaying;

		/// <summary>
		/// Creates an instance of the <c>MultiXmlTextWriter</c> class using the specified filename and
		/// encoding.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter"/>.
		/// Overridden to set output directory.
		/// </summary>
		/// <param name="fileName">The filename to write to. If the file exists, it will truncate it and overwrite it 
		/// with the new content.</param>
		/// <param name="encoding">The encoding to generate. If encoding is a null reference it writes the file out 
		/// as UTF-8, and omits the encoding attribute from the ProcessingInstruction.</param>
		/// <exception cref="ArgumentException">The encoding is not supported; the filename is empty, contains only 
		/// white space, or contains one or more invalid characters.</exception>
		/// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
		/// <exception cref="ArgumentNullException">The filename is a null reference.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
		/// <exception cref="IOException">The filename includes an incorrect or invalid syntax 
		/// for file name, directory name, or volume label syntax.</exception>
		/// <exception cref="SecurityException">The caller does not have the required permission.</exception>
		public MultiXmlTextWriter(string fileName, Encoding encoding)
			: base(fileName, encoding)
		{
			outResolver = new OutputResolver(Path.GetDirectoryName(fileName));
		}


		/// <summary>
		/// Creates an instance of the <c>MultiXmlTextWriter</c> class using the specified 
		/// <c>TextWriter</c>, see <see cref="TextWriter"/>.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter"/>.
		/// </summary>
		/// <param name="w">The <c>TextWriter</c> to write to. It is assumed that the <c>TextWriter</c> is 
		/// already set to the correct encoding.</param>
		public MultiXmlTextWriter(TextWriter w) : base(w) { }

		/// <summary>
		/// Creates an instance of the <c>MultiXmlTextWriter</c> class using the specified 
		/// <see cref="TextWriter"/> and <see cref="XmlResolver"/>.
		/// Inherited from <see cref="XmlTextWriter"/>.
		/// </summary>
		/// <param name="w">The <see cref="TextWriter"/> to write to. It is assumed that the 
		/// <see cref="TextWriter"/> is 
		/// already set to the correct encoding.</param>
		/// <param name="outResolver">The resolver to use to retrieve external resources.</param>
		public MultiXmlTextWriter(TextWriter w, XmlResolver outResolver)
			: base(w)
		{
			this.outResolver = outResolver;
		}

		/// <summary>
		/// Creates an instance of the <c>MultiXmlTextWriter</c> class using the specified 
		/// stream and encoding.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter"/>.
		/// </summary>
		/// <param name="w">The stream to which you want to write.</param>
		/// <param name="encoding">The encoding to generate. If encoding is a null 
		/// reference it writes out the stream as UTF-8 and omits the encoding attribute 
		/// from the ProcessingInstruction.</param>
		/// <exception cref="ArgumentException">The encoding is not supported or the stream 
		/// cannot be written to.</exception>
		/// <exception cref="ArgumentNullException">w is a null reference.</exception>
		public MultiXmlTextWriter(Stream w, Encoding encoding) : base(w, encoding) { }

		/// <summary>
		/// Creates an instance of the <c>MultiXmlTextWriter</c> class using the specified 
		/// stream, encoding and resolver.
		/// </summary>
		/// <param name="w">The stream to which you want to write.</param>
		/// <param name="encoding">The encoding to generate. If encoding is a null 
		/// reference it writes out the stream as UTF-8 and omits the encoding attribute 
		/// from the ProcessingInstruction.</param>
		/// <param name="outResolver">The resolver to use to retrieve external resources.</param>
		/// <exception cref="ArgumentException">The encoding is not supported or the stream 
		/// cannot be written to.</exception>
		/// <exception cref="ArgumentNullException">w is a null reference.</exception>
		public MultiXmlTextWriter(Stream w, Encoding encoding, XmlResolver outResolver)
			: base(w, encoding)
		{
			this.outResolver = outResolver;
		}

		/// <summary>
		/// Checks possible start of <c>&lt;exsl:document></c> element content.         
		/// </summary>
		/// <remarks>
		/// When <c>&lt;exsl:document></c> element start tag is detected, the beginning of the 
		/// element's content might be detected as any next character data (not attribute
		/// value though), element start tag, processing instruction or comment.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown when <c>href</c> attribute is absent.</exception>
		/// <exception cref="ArgumentException">Thrown when a document, specified by <c>href</c> attribute is
		/// opened alreary. Two nested <c>&lt;exsl:document></c></exception> elements cannot specify the same 
		/// output URI in their <c>href</c> attributes.
		private void CheckContentStart()
		{
			if (redirectState == RedirectState.WritingRedirectElementAttrs)
			{
				//Check required href attribute
				if (state.Href == null)
				{
				    throw new ArgumentNullException("'href' attribute of exsl:document element must be specified.");
				}

			    //Are we writing to this URI already?
				foreach (OutputState nestedState in states)
				{
				    if (nestedState.Href == state.Href)
				    {
				        throw new ArgumentException("Cannot write to " + state.Href + " two documents simultaneously.");
				    }
				}

			    state.InitWriter(outResolver);
				redirectState = RedirectState.Redirecting;
			}
		}
		/// <summary>
		/// Writes the specified start tag and associates it with the given namespace and prefix.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteStartElement"/>
		/// Overridden to detect <c>exsl:document</c> element start tag.
		/// </summary>        
		/// <param name="prefix">The namespace prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI to associate with the element. If this namespace 
		/// is already in scope and has an associated prefix then the writer will automatically write that prefix also. </param>
		/// <exception cref="InvalidOperationException">The writer is closed.</exception>
		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			CheckContentStart();
			//Is it exsl:document redirecting instruction?
			if (ns == RedirectNamespace && localName == RedirectElementName)
			{
				//Lazy stack of states
				if (states == null)
				{
				    states = new Stack();
				}

			    //If we are redirecting already - push the current state into the stack
				if (redirectState == RedirectState.Redirecting)
				{
				    states.Push(state);
				}

			    //Initialize new state
				state = new OutputState();
				redirectState = RedirectState.WritingRedirectElementAttrs;
			}
			else
			{
				if (redirectState == RedirectState.Redirecting)
				{
					if (state.Method == OutputMethod.Text)
					{
						state.Depth++;
						return;
					}
					//Write doctype before the first element
					if (state.Depth == 0 && state.SystemDoctype != null)
					{
					    if (prefix != string.Empty)
					    {
					        state.XmlWriter.WriteDocType(prefix + ":" + localName,
					            state.PublicDoctype, state.SystemDoctype, null);
					    }
					    else
					    {
					        state.XmlWriter.WriteDocType(localName,
					            state.PublicDoctype, state.SystemDoctype, null);
					    }
					}

				    state.XmlWriter.WriteStartElement(prefix, localName, ns);
					state.Depth++;
				}
				else
				{
				    base.WriteStartElement(prefix, localName, ns);
				}
			}
		}

		/// <summary>
		/// Finishes output redirecting - closes current writer 
		/// and pops previous state.
		/// </summary>
		internal void FinishRedirecting()
		{
			state.CloseWriter();
			//Pop previous state if it exists
			if (states.Count != 0)
			{
				state = (OutputState)states.Pop();
				redirectState = RedirectState.Redirecting;
			}
			else
			{
				state = null;
				redirectState = RedirectState.Relaying;
			}
		}

		/// <summary>
		/// Closes one element and pops the corresponding namespace scope.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteEndElement"/>
		/// Overridden to detect <c>exsl:document</c> element end tag.
		/// </summary>                
		public override void WriteEndElement()
		{
			CheckContentStart();
			if (redirectState == RedirectState.Redirecting)
			{
				//Check if that's exsl:document end tag
				if (state.Depth-- == 0)
				{
				    FinishRedirecting();
				}
				else
				{
					if (state.Method == OutputMethod.Text)
					{
					    return;
					}

				    state.XmlWriter.WriteEndElement();
				}
			}
			else
			{
			    base.WriteEndElement();
			}
		}

		/// <summary>
		/// Closes one element and pops the corresponding namespace scope.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteFullEndElement"/>
		/// Overridden to detect <c>exsl:document</c> element end tag.
		/// </summary>                
		public override void WriteFullEndElement()
		{
			CheckContentStart();
			if (redirectState == RedirectState.Redirecting)
			{
				//Check if it's exsl:document end tag
				if (state.Depth-- == 0)
				{
				    FinishRedirecting();
				}
				else
				{
					if (state.Method == OutputMethod.Text)
					{
					    return;
					}

				    state.XmlWriter.WriteFullEndElement();
				}
			}
			else
			{
			    base.WriteFullEndElement();
			}
		}

		/// <summary>
		/// Writes the start of an attribute.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteStartAttribute"/>
		/// Overridden to detect <c>exsl:document</c> attribute names and to redirect
		/// the output.
		/// </summary>
		/// <param name="prefix">Namespace prefix of the attribute.</param>
		/// <param name="localName">Local name of the attribute.</param>
		/// <param name="ns">Namespace URI of the attribute.</param>                                            
		/// <exception cref="ArgumentException"><c>localName</c>c> is either a null reference or <c>String.Empty</c>.</exception>
		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (redirectState == RedirectState.WritingRedirectElementAttrs)
			{
				redirectState = RedirectState.WritingRedirectElementAttrValue;
				currentAttributeName = localName;
			}
			else if (redirectState == RedirectState.Redirecting)
			{
				if (state.Method == OutputMethod.Text)
				{
				    return;
				}

			    state.XmlWriter.WriteStartAttribute(prefix, localName, ns);
			}
			else
			{
			    base.WriteStartAttribute(prefix, localName, ns);
			}
		}

		/// <summary>
		/// Closes the previous <c>WriteStartAttribute</c> call.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteEndAttribute"/>
		/// Overridden to redirect the output.
		/// </summary>        
		public override void WriteEndAttribute()
		{
			if (redirectState == RedirectState.WritingRedirectElementAttrValue)
			{
			    redirectState = RedirectState.WritingRedirectElementAttrs;
			}
			else if (redirectState == RedirectState.Redirecting)
			{
				if (state.Method == OutputMethod.Text)
				{
				    return;
				}

			    state.XmlWriter.WriteEndAttribute();
			}
			else
			{
			    base.WriteEndAttribute();
			}
		}

		/// <summary>
		/// Writes out a comment &lt;!--...--> containing the specified text.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteComment"/>
		/// Overriden to redirect the output.
		/// </summary>
		/// <param name="text">Text to place inside the comment.</param>        
		/// <exception cref="ArgumentException">The text would result in a non-well formed XML document.</exception>
		/// <exception cref="InvalidOperationException">The <c>WriteState</c> is Closed.</exception>
		public override void WriteComment(string text)
		{
			CheckContentStart();
			if (redirectState == RedirectState.Redirecting)
			{
				if (state.Method == OutputMethod.Text)
				{
				    return;
				}

			    state.XmlWriter.WriteComment(text);
			}
			else
			{
			    base.WriteComment(text);
			}
		}

		/// <summary>
		/// Writes out a processing instruction with a space between the name 
		/// and text as follows: &lt;?name text?>.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteProcessingInstruction"/>
		/// Overridden to redirect the output.
		/// </summary>
		/// <param name="name">Name of the processing instruction.</param>
		/// <param name="text">Text to include in the processing instruction.</param>        
		/// <exception cref="ArgumentException"><para>The text would result in a non-well formed XML document.</para> 
		/// <para><c>name</c> is either a null reference or <c>String.Empty</c>.</para>
		/// <para>This method is being used to create an XML declaration after 
		/// <c>WriteStartDocument</c> has already been called.</para></exception>
		public override void WriteProcessingInstruction(string name, string text)
		{
			CheckContentStart();
			if (redirectState == RedirectState.Redirecting)
			{
				if (state.Method == OutputMethod.Text)
				{
				    return;
				}

			    state.XmlWriter.WriteProcessingInstruction(name, text);
			}
			else
			{
			    base.WriteProcessingInstruction(name, text);
			}
		}

		/// <summary>
		/// Writes the given text content.
		/// Inherited from <c>XmlTextWriter</c>, see <see cref="XmlTextWriter.WriteString"/>
		/// Overridden to detect <c>exsl:document</c> element attribute values and to 
		/// redirect the output.
		/// </summary>
		/// <param name="text">Text to write.</param>        
		/// <exception cref="ArgumentException">The text string contains an invalid surrogate pair.</exception>
		public override void WriteString(string text)
		{
			//Possible exsl:document's attribute value
			if (redirectState == RedirectState.WritingRedirectElementAttrValue)
			{
				switch (currentAttributeName)
				{
					case "href":
						state.Href += text;
						break;
					case "method":
						if (text == "text")
						{
						    state.Method = OutputMethod.Text;
						}

					    break;
					case "encoding":
					    try
					    {
					        state.Encoding = Encoding.GetEncoding(text);
					    }
					    catch (Exception)
					    {
                            // swallow
					    }
						break;
					case "indent":
						if (text == "yes")
						{
						    state.Indent = true;
						}

					    break;
					case "doctype-public":
						state.PublicDoctype = text;
						break;
					case "doctype-system":
						state.SystemDoctype = text;
						break;
					case "standalone":
						if (text == "yes")
						{
						    state.Standalone = true;
						}

					    break;
					case "omit-xml-declaration":
						if (text == "yes")
						{
						    state.OmitXmlDeclaration = true;
						}

					    break;
				}
				return;
			}

		    CheckContentStart();

		    if (redirectState == RedirectState.Redirecting)
			{
				if (state.Method == OutputMethod.Text)
				{
				    state.TextWriter.Write(text);
				}
				else
				{
				    state.XmlWriter.WriteString(text);
				}
			}
			else
		    {
		        base.WriteString(text);
		    }
		}
	}
}
