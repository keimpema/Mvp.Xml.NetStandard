using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Common.Xsl
{

	/// <summary>
	/// <para>XslReader provides an efficient way to read results of an XSL 
	/// transformation via an <see cref="XmlReader"/> API. Due to 
	/// architectural and performance reasons the <see cref="XslCompiledTransform"/>
	/// class doesn't support transforming to an <see cref="XmlReader"/> as obsolete 
	/// <see cref="XslTransform"/> class did and XslReader's goal is to 
	/// supplement such functionality.</para>
	/// <para>XslReader has been developed and contributed to the Mvp.Xml project
	/// by Sergey Dubinets (Microsoft XML Team).</para>
	/// </summary>    
	/// <remarks>
	/// <para>XslReader can work in a singlethreaded (fully buffering) or a
	/// multithreaded mode.</para> 
	/// <para>In a multithreaded mode XslReader runs an XSL transformation 
	/// in a separate dedicated thread. XSLT output is being recorded into a buffer 
	/// and once the buffer is full, transformation thread gets suspended. In a main
	/// thread XslReader reads recorded XSLT output from a buffer as a client calls 
	/// XslReader methods. Whenever the buffer is read, the transformation thread
	/// is resumed to produce next portion of an XSLT output.<br/>
	/// In effect that means that an XSL transformation happens on demand portion by 
	/// portion as a client calls XslReader methods. In terms of memory footprint
	/// that means that at any time at most buffer size of XSLT output is buffered.</para>
	/// <para>In a singlethreaded mode XslReader runs an XSL transformation
	/// to the end and records full XSLT output into a buffer (using effective 
	/// binary representation though). After that it reads the buffer when a client 
	/// calls XslReader methods. So in this mode before first call to the
	/// XslReader.Read() method returns, XSL transformation is over and XSLT output 
	/// is buffered internally as a whole.    
	/// </para>
	/// <para>By default XslReader works in a multithreaded mode. You can choose the mode
	/// and the buffer size using <c>multiThread</c> and <c>initialBufferSize</c> arguments
	/// when instantiating XslReader object. On small XSLT outputs XslReader performs
	/// better in a singlethreaded mode, but on medium and big outputs multithreaded 
	/// mode is preferrable. You are adviced to measure performance in both modes to
	/// find out which suites better for your particular scenario.</para>
	/// <para>XslReader designed to be reused. Just provide another inoput XML or XSLT
	/// stylesheet, start transformation and read the output. If the <c>StartTransform()</c> 
	/// method is called when previous
	/// transformation isn't over yet, it will be aborted, the buffer cleaned and 
	/// the XslReader object will be reset to an initial state automatically.</para>
	/// <para>XslReader is not thread safe, keep separate instances of the XslReader
	/// for each thread.</para>
	/// </remarks>
	/// <example>
	/// <para>Here is an example of using XslReader class. First you need to create
	/// an <see cref="XslCompiledTransform"/> object and load XSLT stylesheet you want to
	/// execute. Then prepare XML input as <see cref="XmlInput"/> object providing
	/// XML source in a form of URI, <see cref="Stream"/>, <see cref="TextReader"/>, 
	/// <see cref="XmlReader"/> or <see cref="IXPathNavigable"/> along with an optional 
	/// <see cref="XmlResolver"/> object, which will be used to resolve URIs for 
	/// the XSLT document() function calls.<br/>
	/// After that create XslReader instance optionally choosing multithreaded or
	/// singlethreaded mode and initial buffer size.<br/>
	/// Finally start transformation by calling <c>StartTransform()</c> method and then
	/// you can read transformation output via XslReader object, which implements 
	/// <see cref="XmlReader"/> API.
	/// </para> 
	/// <para>
	/// Basic XslReader usage sample:
	/// <code>
	/// //Prepare XslCompiledTransform
	/// XslCompiledTransform xslt = new XslCompiledTransform();
	/// xslt.Load("catalog.xslt");
	/// //Prepare input XML
	/// XmlInput input = new XmlInput("books.xml");
	/// //Create XslReader
	/// XslReader xslReader = new XslReader(xslt);
	/// //Initiate transformation
	/// xslReader.StartTransform(input, null);
	/// //Now read XSLT output from the reader
	/// XPathDocument results = new XPathDocument(xslReader);
	/// </code>
	/// A more advanced sample:
	/// <code>
	/// //Prepare XslCompiledTransform
	/// XslCompiledTransform xslt = new XslCompiledTransform();
	/// xslt.Load("../../catalog.xslt");
	/// //Prepare XmlResolver to be used by the document() function
	/// XmlResolver resolver = new XmlUrlResolver();
	/// resolver.Credentials = new NetworkCredential("user42", "god");
	/// //Prepare input XML
	/// XmlInput input = new XmlInput("../../books.xml", resolver);
	/// //Create XslReader, multithreaded mode, initial buffer for 32 nodes
	/// XslReader xslReader = new XslReader(xslt, true, 32);
	/// //XSLT parameters
	/// XsltArgumentList prms = new XsltArgumentList();
	/// prms.AddParam("param2", "", "red");
	/// //Initiate transformation
	/// xslReader.StartTransform(input, prms);
	/// //Now read XSLT output from the reader
	/// XPathDocument results = new XPathDocument(xslReader);
	/// </code>
	/// </para>
	/// </example>
	public class XslReader : XmlReader
	{
		private const string nsXml = "http://www.w3.org/XML/1998/namespace";
		private const string nsXmlNs = "http://www.w3.org/2000/xmlns/";
		private const int defaultBufferSize = 256;
		private readonly XmlNameTable nameTable;
		private readonly TokenPipe pipe;
		private readonly BufferWriter writer;
		private readonly ScopeManager scope;
		private Thread thread;

		private readonly bool multiThread;

		private static readonly XmlReaderSettings readerSettings;
		static XslReader()
		{
			readerSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };
			//ReaderSettings.ProhibitDtd = true;

		}

		// Transform Parameters
		private XmlInput defaulDocument;
		private XsltArgumentList arguments;

		/// <summary>
		/// Creates new XslReader instance with given <see cref="XslCompiledTransform"/>, 
		/// mode (multithreaded/singlethreaded) and initial buffer size. The buffer will be
		/// expanded if necessary to be able to store any element start tag with all its 
		/// attributes.
		/// </summary>
		/// <param name="xslTransform">Loaded <see cref="XslCompiledTransform"/> object</param>
		/// <param name="multiThread">Defines in which mode (multithreaded or singlethreaded)
		/// this instance of XslReader will operate</param>
		/// <param name="initialBufferSize">Initial buffer size (number of nodes, not bytes)</param>
		public XslReader(XslCompiledTransform xslTransform, bool multiThread, int initialBufferSize)
		{
			XslCompiledTransform = xslTransform;
			this.multiThread = multiThread;
			InitialBufferSize = initialBufferSize;

			nameTable = new NameTable();
			pipe = this.multiThread ? new TokenPipeMultiThread(initialBufferSize) : new TokenPipe(initialBufferSize);
			writer = new BufferWriter(pipe, nameTable);
			scope = new ScopeManager(nameTable);
			SetUndefinedState(ReadState.Initial);
		}

		/// <summary>
		/// Creates new XslReader instance with given <see cref="XslCompiledTransform"/>,
		/// operating in a multithreaded mode and having default initial buffer size.
		/// </summary>
		/// <param name="xslTransform">Loaded <see cref="XslCompiledTransform"/> object</param>
		public XslReader(XslCompiledTransform xslTransform) : this(xslTransform, true, defaultBufferSize) { }

		/// <summary>
		/// Starts XSL transformation of given <see cref="XmlInput"/> object with
		/// specified <see cref="XsltArgumentList"/>. After this method returns
		/// you can read the transformation output out of XslReader object via 
		/// standard <see cref="XmlReader"/> methods such as Read() or MoveXXX().
		/// </summary>
		/// <remarks>If the <c>StartTransform()</c> method is called when previous
		/// transformation isn't over yet, it will be aborted, buffer cleaned and 
		/// XslReader object reset to an initial state automatically.</remarks>
		/// <param name="input">An input XML to be transformed</param>
		/// <param name="args">A collection of global parameter values and
		/// extension objects.</param>
		/// <returns></returns>
		public XmlReader StartTransform(XmlInput input, XsltArgumentList args)
		{
			defaulDocument = input;
			arguments = args;
			Start();
			return this;
		}

		private void Start()
		{
			if (thread != null && thread.IsAlive)
			{
				// We can also reuse this thread or use ThreadPool. For simplicity we create new thread each time.
				// Some problem with TreadPool will be the need to notify transformation thread when user calls new Start() befor previous transformation completed
				thread.Interrupt();
				thread.Join();
			}
			writer.Reset();
			scope.Reset();
			pipe.Reset();
			depth = 0;
			SetUndefinedState(ReadState.Initial);
			if (multiThread)
			{
				principal = Thread.CurrentPrincipal;
				thread = new Thread(StartTransform);
				thread.Start();
			}
			else
			{
				StartTransform();
			}
		}

		private void StartTransform()
		{
			try
			{
				Thread.CurrentPrincipal = principal;

				while (true)
				{
					if (defaulDocument.Source is XmlReader xmlReader)
					{
						XslCompiledTransform.Transform(xmlReader, arguments, writer, defaulDocument.Resolver);
						break;
					}

					if (defaulDocument.Source is IXPathNavigable nav)
					{
						XslCompiledTransform.Transform(nav, arguments, writer);
						break;
					}

					if (defaulDocument.Source is string str)
					{
						using (XmlReader reader = Create(str, readerSettings))
						{
							XslCompiledTransform.Transform(reader, arguments, writer, defaulDocument.Resolver);
						}
						break;
					}

					if (defaulDocument.Source is Stream strm)
					{
						using (XmlReader reader = Create(strm, readerSettings))
						{
							XslCompiledTransform.Transform(reader, arguments, writer, defaulDocument.Resolver);
						}
						break;
					}

					if (defaulDocument.Source is TextReader txtReader)
					{
						using (XmlReader reader = Create(txtReader, readerSettings))
						{
							XslCompiledTransform.Transform(reader, arguments, writer, defaulDocument.Resolver);
						}
						break;
					}
					throw new Exception("Unexpected XmlInput");
				}
				writer.Close();
			}
			catch (Exception e)
			{
				if (multiThread)
				{
					// we need this exception on main thread. So pass it through pipe.
					pipe.WriteException(e);
				}
				else
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Loaded <see cref="XslCompiledTransform"/> object, which is used
		/// to run XSL transformations. You can reuse XslReader for running
		/// another transformation by replacing <see cref="XslCompiledTransform"/> 
		/// object.
		/// </summary>
		public XslCompiledTransform XslCompiledTransform { get; set; }

		/// <summary>
		/// Initial buffer size. The buffer will be
		/// expanded if necessary to be able to store any element start tag with 
		/// all its attributes.
		/// </summary>
		public int InitialBufferSize { get; set; }

		private int attOffset; // 0 - means reader is positioned on element, when reader potitionrd on the first attribute attOffset == 1
		private int attCount;
		private int depth;
		private XmlNodeType nodeType = XmlNodeType.None;
		private ReadState readState = ReadState.Initial;
		private QName qname;
		private string value;

		private void SetUndefinedState(ReadState readState)
		{
			qname = writer.QNameEmpty;
			value = string.Empty;
			nodeType = XmlNodeType.None;
			attCount = 0;
			this.readState = readState;
		}

		private static bool IsWhitespace(string s)
		{
			// Because our xml is presumably valid only and all ws chars <= ' '
			foreach (char c in s)
			{
				if (' ' < c)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// See <see cref="XmlReader.Read()"/>.
		/// </summary>        
		public override bool Read()
		{
			// Leave Current node
			switch (nodeType)
			{
				case XmlNodeType.None:
					if (readState == ReadState.EndOfFile || readState == ReadState.Closed)
					{
						return false;
					}
					readState = ReadState.Interactive;
					break;
				case XmlNodeType.Attribute:
					attOffset = 0;
					depth--;
					goto case XmlNodeType.Element;
				case XmlNodeType.Element:
					pipe.FreeTokens(1 + attCount);
					depth++;
					break;
				case XmlNodeType.EndElement:
					scope.PopScope();
					pipe.FreeTokens(1);
					break;
				case XmlNodeType.Text:
					if (attOffset != 0)
					{
						// We are on text node inside of the attribute
						attOffset = 0;
						depth -= 2;
						goto case XmlNodeType.Element;
					}
					pipe.FreeTokens(1);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.SignificantWhitespace:
				case XmlNodeType.Whitespace:
					pipe.FreeTokens(1);
					break;
				default:
					throw new InvalidProgramException("Internal Error: unexpected node type");
			}
			Debug.Assert(attOffset == 0);
			Debug.Assert(readState == ReadState.Interactive);
			attCount = 0;
			// Step on next node
			pipe.Read(out nodeType, out qname, out value);
			if (nodeType == XmlNodeType.None)
			{
				SetUndefinedState(ReadState.EndOfFile);
				return false;
			}

			switch (nodeType)
			{
				case XmlNodeType.Element:
					for (attCount = 0; ; attCount++)
					{
						pipe.Read(out XmlNodeType attType, out QName attName, out string attText);
						if (attType != XmlNodeType.Attribute)
						{
							break; // We are done with attributes for this element
						}
						if (RefEquals(attName.Prefix, "xmlns")) { scope.AddNamespace(attName.Local, attText); }
						else if (RefEquals(attName, writer.QNameXmlNs)) { scope.AddNamespace(attName.Prefix, attText); }  // prefix is atomized empty string
						else if (RefEquals(attName, writer.QNameXmlLang)) { scope.AddLang(attText); }
						else if (RefEquals(attName, writer.QNameXmlSpace)) { scope.AddSpace(attText); }
					}
					scope.PushScope(qname);
					break;
				case XmlNodeType.EndElement:
					qname = scope.Name;
					depth--;
					break;
				case XmlNodeType.Comment:
				case XmlNodeType.ProcessingInstruction:
					break;
				case XmlNodeType.Text:
					if (IsWhitespace(value))
					{
						nodeType = XmlSpace == XmlSpace.Preserve ? XmlNodeType.SignificantWhitespace : XmlNodeType.Whitespace;
					}
					break;
				default:
					throw new InvalidProgramException("Internal Error: unexpected node type");
			}
			return true;
		}

		/// <summary>See <see cref="XmlReader.AttributeCount"/>.</summary>
		public override int AttributeCount => attCount;

		// issue: What should be BaseURI in XslReader? xslCompiledTransform.BaseURI ?
		/// <summary>See <see cref="XmlReader.BaseURI"/>.</summary>
		public override string BaseURI => string.Empty;

		/// <summary>See <see cref="XmlReader.NameTable"/>.</summary>
		public override XmlNameTable NameTable => nameTable;

		/// <summary>See <see cref="XmlReader.Depth"/>.</summary>
		public override int Depth => depth;

		/// <summary>See <see cref="XmlReader.EOF"/>.</summary>
		public override bool EOF => ReadState == ReadState.EndOfFile;

		/// <summary>See <see cref="XmlReader.HasValue"/>.</summary>
		public override bool HasValue => 0 != (/*HasValueBitmap:*/0x2659C & (1 << (int)nodeType));

		/// <summary>See <see cref="XmlReader.NodeType"/>.</summary>
		public override XmlNodeType NodeType => nodeType;

		// issue: We may want return true if element doesn't have content. Iteresting to know what 
		/// <summary>See <see cref="XmlReader.IsEmptyElement"/>.</summary>
		public override bool IsEmptyElement => false;

		/// <summary>See <see cref="XmlReader.LocalName"/>.</summary>
		public override string LocalName => qname.Local;

		/// <summary>See <see cref="XmlReader.NamespaceURI"/>.</summary>
		public override string NamespaceURI => qname.NsUri;

		/// <summary>See <see cref="XmlReader.Prefix"/>.</summary>
		public override string Prefix => qname.Prefix;

		/// <summary>See <see cref="XmlReader.Value"/>.</summary>
		public override string Value => value;

		/// <summary>See <see cref="XmlReader.ReadState"/>.</summary>
		public override ReadState ReadState => readState;

		/// <summary>See <see cref="XmlReader.Close()"/>.</summary>
		public override void Close()
		{
			SetUndefinedState(ReadState.Closed);
		}
		/// <summary>See <see cref="XmlReader.GetAttribute(int)"/>.</summary>
		public override string GetAttribute(int i)
		{
			if (IsInsideElement())
			{
				if (0 <= i && i < attCount)
				{
					pipe.GetToken(i + 1, out _, out string attValue);
					return attValue;
				}
			}
			throw new ArgumentOutOfRangeException("i");
		}

		private static readonly char[] qnameSeparator = { ':' };
		private IPrincipal principal;

		private int FindAttribute(string name)
		{
			if (IsInsideElement())
			{
				string prefix, local;
				string[] strings = name.Split(qnameSeparator, StringSplitOptions.None);
				switch (strings.Length)
				{
					case 1:
						prefix = string.Empty;
						local = name;
						break;
					case 2:
						if (strings[0].Length == 0)
						{
							return 0; // ":local-name"
						}
						prefix = strings[0];
						local = strings[1];
						break;
					default:
						return 0;
				}
				for (int i = 1; i <= attCount; i++)
				{
					pipe.GetToken(i, out QName attName, out _);
					if (attName.Local == local && attName.Prefix == prefix)
					{
						return i;
					}
				}
			}
			return 0;
		}
		/// <summary>See <see cref="XmlReader.GetAttribute(string)"/>.</summary>
		public override string GetAttribute(string name)
		{
			int attNum = FindAttribute(name);
			if (attNum != 0)
			{
				return GetAttribute(attNum - 1);
			}
			return null;
		}
		/// <summary>See <see cref="XmlReader.GetAttribute(string, string)"/>.</summary>
		public override string GetAttribute(string name, string ns)
		{
			if (IsInsideElement())
			{
				for (int i = 1; i <= attCount; i++)
				{
					pipe.GetToken(i, out QName attName, out string attValue);
					if (attName.Local == name && attName.NsUri == ns)
					{
						return attValue;
					}
				}
			}
			return null;
		}
		/// <summary>See <see cref="XmlReader.LookupNamespace(string)"/>.</summary>
		public override string LookupNamespace(string prefix) { return scope.LookupNamespace(prefix); }
		/// <summary>See <see cref="XmlReader.Close()"/>.</summary>
		public override bool MoveToAttribute(string name)
		{
			int attNum = FindAttribute(name);
			if (attNum != 0)
			{
				MoveToAttribute(attNum - 1);
				return true;
			}
			return false;
		}
		/// <summary>See <see cref="XmlReader.MoveToAttribute(int)"/>.</summary>
		public override void MoveToAttribute(int i)
		{
			if (IsInsideElement())
			{
				if (0 <= i && i < attCount)
				{
					ChangeDepthToElement();
					attOffset = i + 1;
					depth++;
					pipe.GetToken(attOffset, out qname, out value);
					nodeType = XmlNodeType.Attribute;
					return;
				}
			}
			throw new ArgumentOutOfRangeException("i");
		}
		/// <summary>See <see cref="XmlReader.MoveToAttribute(string, string)"/>.</summary>
		public override bool MoveToAttribute(string name, string ns)
		{
			if (IsInsideElement())
			{
				for (int i = 1; i <= attCount; i++)
				{
					pipe.GetToken(i, out QName attName, out string attValue);
					if (attName.Local == name && attName.NsUri == ns)
					{
						ChangeDepthToElement();
						nodeType = XmlNodeType.Attribute;
						attOffset = i;
						qname = attName;
						depth++;
						value = attValue;
					}
				}
			}
			return false;
		}
		private bool IsInsideElement()
		{
			return (
				nodeType == XmlNodeType.Element ||
				nodeType == XmlNodeType.Attribute ||
				nodeType == XmlNodeType.Text && attOffset != 0
			);
		}
		private void ChangeDepthToElement()
		{
			switch (nodeType)
			{
				case XmlNodeType.Attribute:
					depth--;
					break;
				case XmlNodeType.Text:
					if (attOffset != 0)
					{
						depth -= 2;
					}
					break;
			}
		}
		/// <summary>See <see cref="XmlReader.MoveToElement()"/>.</summary>
		public override bool MoveToElement()
		{
			if (
				nodeType == XmlNodeType.Attribute ||
				nodeType == XmlNodeType.Text && attOffset != 0
			)
			{
				ChangeDepthToElement();
				nodeType = XmlNodeType.Element;
				attOffset = 0;
				pipe.GetToken(0, out qname, out value);
				return true;
			}
			return false;
		}
		/// <summary>See <see cref="XmlReader.MoveToFirstAttribute()"/>.</summary>
		public override bool MoveToFirstAttribute()
		{
			ChangeDepthToElement();
			attOffset = 0;
			return MoveToNextAttribute();
		}
		/// <summary>See <see cref="XmlReader.MoveToNextAttribute()"/>.</summary>
		public override bool MoveToNextAttribute()
		{
			if (attOffset < attCount)
			{
				ChangeDepthToElement();
				depth++;
				attOffset++;
				pipe.GetToken(attOffset, out qname, out value);
				nodeType = XmlNodeType.Attribute;
				return true;
			}
			return false;
		}
		/// <summary>See <see cref="XmlReader.ReadAttributeValue()"/>.</summary>
		public override bool ReadAttributeValue()
		{
			if (nodeType == XmlNodeType.Attribute)
			{
				nodeType = XmlNodeType.Text;
				depth++;
				return true;
			}
			return false;
		}
		/// <summary>See <see cref="XmlReader.ResolveEntity()"/>.</summary>
		public override void ResolveEntity() { throw new InvalidOperationException(); }

		/// <summary>See <see cref="XmlReader.XmlLang"/>.</summary>
		public override string XmlLang => scope.Lang;

		/// <summary>See <see cref="XmlReader.XmlSpace"/>.</summary>
		public override XmlSpace XmlSpace => scope.Space;

		private static bool RefEquals(string strA, string strB)
		{
			Debug.Assert(
				ReferenceEquals(strA, strB) || !string.Equals(strA, strB),
				"String atomization Failure: '" + strA + "'"
			);
			return ReferenceEquals(strA, strB);
		}

		private static bool RefEquals(QName qnA, QName qnB)
		{
			Debug.Assert(
				ReferenceEquals(qnA, qnB) || qnA.Local != qnB.Local || qnA.NsUri != qnB.NsUri || qnA.Prefix != qnB.Prefix,
				"QName atomization Failure: '" + qnA + "'"
			);
			return ReferenceEquals(qnA, qnB);
		}

		// BufferWriter records information written to it in sequence of WriterEvents: 
		[DebuggerDisplay("{NodeType}: name={Name}, Value={Value}")]
		private struct XmlToken
		{
			public XmlNodeType NodeType;
			public QName Name;
			public string Value;

			// it seams that it faster to set fields of structure in one call.
			// This trick is workaround of the C# limitation of declaring variable as ref to a struct.
			public static void Set(ref XmlToken evnt, XmlNodeType nodeType, QName name, string value)
			{
				evnt.NodeType = nodeType;
				evnt.Name = name;
				evnt.Value = value;
			}
			public static void Get(ref XmlToken evnt, out XmlNodeType nodeType, out QName name, out string value)
			{
				nodeType = evnt.NodeType;
				name = evnt.Name;
				value = evnt.Value;
			}
		}

		private class BufferWriter : XmlWriter
		{
			private readonly QNameTable qnameTable;
			private readonly TokenPipe pipe;
			private string firstText;
			private readonly StringBuilder sbuilder;
			private QName curAttribute;

			public readonly QName QNameXmlSpace;
			public readonly QName QNameXmlLang;
			public readonly QName QNameEmpty;
			public readonly QName QNameXmlNs;

			public BufferWriter(TokenPipe pipe, XmlNameTable nameTable)
			{
				this.pipe = pipe;
				qnameTable = new QNameTable(nameTable);
				sbuilder = new StringBuilder();
				QNameXmlSpace = qnameTable.GetQName("space", nsXml, "xml"); // xml:space
				QNameXmlLang = qnameTable.GetQName("lang", nsXml, "xml"); // xml:lang
				QNameXmlNs = qnameTable.GetQName("xmlns", nsXmlNs, ""); // xmlsn=""
				QNameEmpty = qnameTable.GetQName("", "", "");
			}

			public void Reset()
			{
				firstText = null;
				sbuilder.Length = 0;
			}

			private void AppendText(string text)
			{
				if (firstText == null)
				{
					Debug.Assert(sbuilder.Length == 0);
					firstText = text;
				}
				else if (sbuilder.Length == 0)
				{
					sbuilder.Append(firstText);
				}
				sbuilder.Append(text);
			}
			private string MergeText()
			{
				if (firstText == null)
				{
					return string.Empty; // There was no text ouptuted
				}
				if (sbuilder.Length != 0)
				{
					// merge content of sbuilder into firstText
					Debug.Assert(firstText != null);
					firstText = sbuilder.ToString();
					sbuilder.Length = 0;
				}
				string result = firstText;
				firstText = null;
				return result;
			}

			private void FinishTextNode()
			{
				string text = MergeText();
				if (text.Length != 0)
				{
					pipe.Write(XmlNodeType.Text, QNameEmpty, text);
				}
			}

			public override void WriteComment(string text)
			{
				FinishTextNode();
				pipe.Write(XmlNodeType.Comment, QNameEmpty, text);
			}

			public override void WriteProcessingInstruction(string name, string text)
			{
				FinishTextNode();
				pipe.Write(XmlNodeType.ProcessingInstruction, qnameTable.GetQName(name, string.Empty, string.Empty), text);
			}

			public override void WriteStartElement(string prefix, string name, string ns)
			{
				FinishTextNode();
				pipe.Write(XmlNodeType.Element, qnameTable.GetQName(name, ns, prefix), "");
			}
			public override void WriteEndElement()
			{
				FinishTextNode();
				pipe.Write(XmlNodeType.EndElement, QNameEmpty, "");
			}
			public override void WriteStartAttribute(string prefix, string name, string ns)
			{
				curAttribute = qnameTable.GetQName(name, ns, prefix);
			}
			public override void WriteEndAttribute()
			{
				pipe.Write(XmlNodeType.Attribute, curAttribute, MergeText());
			}
			public override void WriteString(string text)
			{
				AppendText(text);
			}

			public override void WriteFullEndElement()
			{
				WriteEndElement();
			}
			public override void WriteRaw(string data)
			{
				WriteString(data); // In XslReader output we ignore disable-output-escaping
			}

			public override void Close()
			{
				FinishTextNode();
				pipe.Close();
			}
			public override void Flush() { }

			// XsltCompiledTransform never calls these methods and properties:
			public override void WriteStartDocument() { throw new NotSupportedException(); }
			public override void WriteStartDocument(bool standalone) { throw new NotSupportedException(); }
			public override void WriteEndDocument() { throw new NotSupportedException(); }
			public override void WriteDocType(string name, string pubid, string sysid, string subset) { throw new NotSupportedException(); }
			public override void WriteEntityRef(string name) { throw new NotSupportedException(); }
			public override void WriteCharEntity(char ch) { throw new NotSupportedException(); }
			public override void WriteSurrogateCharEntity(char lowChar, char highChar) { throw new NotSupportedException(); }
			public override void WriteWhitespace(string ws) { throw new NotSupportedException(); }
			public override void WriteChars(char[] buffer, int index, int count) { throw new NotSupportedException(); }
			public override void WriteRaw(char[] buffer, int index, int count) { throw new NotSupportedException(); }
			public override void WriteBase64(byte[] buffer, int index, int count) { throw new NotSupportedException(); }
			public override void WriteCData(string text) { throw new NotSupportedException(); }
			public override string LookupPrefix(string ns) { throw new NotSupportedException(); }
			public override WriteState WriteState => throw new NotSupportedException();
			public override XmlSpace XmlSpace => throw new NotSupportedException();
			public override string XmlLang => throw new NotSupportedException();

			private class QNameTable
			{
				// This class atomizes QNames.
				private readonly XmlNameTable nameTable;
				private readonly Dictionary<string, List<QName>> qnames = new Dictionary<string, List<QName>>();

				public QNameTable(XmlNameTable nameTable)
				{
					this.nameTable = nameTable;
				}

				public QName GetQName(string local, string nsUri, string prefix)
				{
					nsUri = nameTable.Add(nsUri);
					prefix = nameTable.Add(prefix);
					if (!qnames.TryGetValue(local, out List<QName> list))
					{
						list = new List<QName>();
						qnames.Add(local, list);
					}
					else
					{
						foreach (QName qn in list)
						{
							Debug.Assert(qn.Local == local, "Atomization Failure: '" + local + "'");
							if (RefEquals(qn.Prefix, prefix) && RefEquals(qn.NsUri, nsUri))
							{
								return qn;
							}
						}
					}
					QName qname = new QName(nameTable.Add(local), nsUri, prefix);
					list.Add(qname);
					return qname;
				}

				//private static string Atomize(string s, Dictionary<string, string> dic)
				//{
				//    if (dic.TryGetValue(s, out string atom))
				//	{
				//		return atom;
				//	}

				//    dic.Add(s, s);
				//    return s;
				//}
			}
		}

		private class ScopeManager
		{
			// We need the scope for the following reasons:
			// 1. Report QName on EndElement  (local, nsUri, prefix )
			// 2. Keep scope of Namespaces    (null , nsUri, prefix )
			// 3. Keep scope of xml:lang      (null , lang , "lang" )
			// 4. Keep scope of xml:space     (null , space, "space")
			// On each StartElement we adding record(s) to the scope, 
			// Its convinient to add QName last becuase in this case it will be directly available for EndElement
			private static readonly string atomLang = new string("lang".ToCharArray());
			private static readonly string atomSpace = new string("space".ToCharArray());
			private readonly XmlNameTable nameTable;
			private readonly string stringEmpty;
			private QName[] records = new QName[32];
			private int lastRecord;

			public ScopeManager(XmlNameTable nameTable)
			{
				this.nameTable = nameTable;
				stringEmpty = nameTable.Add(string.Empty);
				Lang = stringEmpty;
				Space = XmlSpace.None;
				Reset();
			}

			public void Reset()
			{
				lastRecord = 0;
				records[lastRecord++] = new QName(null, nameTable.Add(nsXml), nameTable.Add("xml"));  // xmlns:xml="http://www.w3.org/XML/1998/namespace"
				records[lastRecord++] = new QName(null, stringEmpty, stringEmpty);                    // xml=""
				records[lastRecord++] = new QName(stringEmpty, stringEmpty, stringEmpty);                    // --  lookup barier
			}

			public void PushScope(QName qname)
			{
				Debug.Assert(qname.Local != null, "Scope is Element Name");
				AddRecord(qname);
			}

			public void PopScope()
			{
				Debug.Assert(records[lastRecord - 1].Local != null, "LastRecord in each scope is expected to be ElementName");
				do
				{
					lastRecord--;
					Debug.Assert(0 < lastRecord, "Push/Pop balance error");
					QName record = records[lastRecord - 1];
					if (record.Local != null)
					{
						break; //  this record is Element QName
					}
					if (RefEquals(record.Prefix, atomLang))
					{
						Lang = record.NsUri;
					}
					else if (RefEquals(record.Prefix, atomSpace))
					{
						Space = Str2Space(record.NsUri);
					}
				} while (true);
			}

			private void AddRecord(QName qname)
			{
				if (lastRecord == records.Length)
				{
					QName[] temp = new QName[records.Length * 2];
					records.CopyTo(temp, 0);
					records = temp;
				}
				records[lastRecord++] = qname;
			}

			public void AddNamespace(string prefix, string uri)
			{
				Debug.Assert(prefix != null);
				Debug.Assert(uri != null);
				Debug.Assert(prefix == nameTable.Add(prefix), "prefixes are expected to be already atomized in this NameTable");
				uri = nameTable.Add(uri);
				Debug.Assert(
					!RefEquals(prefix, atomLang) &&
					!RefEquals(prefix, atomSpace)
				,
					"This assumption is important to distinct NsDecl from xml:space and xml:lang"
				);
				AddRecord(new QName(null, uri, prefix));
			}

			public void AddLang(string lang)
			{
				Debug.Assert(lang != null);
				lang = nameTable.Add(lang);
				if (RefEquals(lang, Lang))
				{
					return;
				}
				AddRecord(new QName(null, Lang, atomLang));
				Lang = lang;
			}

			public void AddSpace(string space)
			{
				Debug.Assert(space != null);
				XmlSpace xmlSpace = Str2Space(space);
				if (xmlSpace == XmlSpace.None)
				{
					throw new Exception("Unexpected value for xml:space attribute");
				}
				if (xmlSpace == Space)
				{
					return;
				}
				AddRecord(new QName(null, Space2Str(Space), atomSpace));
				Space = xmlSpace;
			}
			private string Space2Str(XmlSpace space)
			{
				switch (space)
				{
					case XmlSpace.Preserve: return "preserve";
					case XmlSpace.Default: return "default";
					default: return "none";
				}
			}
			private XmlSpace Str2Space(string space)
			{
				switch (space)
				{
					case "preserve": return XmlSpace.Preserve;
					case "default": return XmlSpace.Default;
					default: return XmlSpace.None;
				}
			}

			public string LookupNamespace(string prefix)
			{
				Debug.Assert(prefix != null);
				prefix = nameTable.Get(prefix);
				for (int i = lastRecord - 2; 0 <= i; i--)
				{
					QName record = records[i];
					if (record.Local == null && RefEquals(record.Prefix, prefix))
					{
						return record.NsUri;
					}
				}
				return null;
			}
			public string Lang { get; private set; }

			public XmlSpace Space { get; private set; }

			public QName Name
			{
				get
				{
					Debug.Assert(records[lastRecord - 1].Local != null, "Element Name is expected");
					return records[lastRecord - 1];
				}
			}
		}

		private class TokenPipe
		{
			protected XmlToken[] Buffer;
			protected int WritePos;                // position after last wrote token
			protected int ReadStartPos;            // 
			protected int ReadEndPos;              // 
			protected int Mask;                    // used in TokenPipeMultiThread

			public TokenPipe(int bufferSize)
			{
				/*BuildMask*/
				{
					if (bufferSize < 2)
					{
						bufferSize = defaultBufferSize;
					}
					// To make or round buffer work bufferSize should be == 2 power N and mask == bufferSize - 1
					bufferSize--;
					Mask = bufferSize;
					while ((bufferSize = bufferSize >> 1) != 0)
					{
						Mask |= bufferSize;
					}
				}
				Buffer = new XmlToken[Mask + 1];
			}

			public virtual void Reset()
			{
				ReadStartPos = ReadEndPos = WritePos = 0;
			}

			public virtual void Write(XmlNodeType nodeType, QName name, string value)
			{
				Debug.Assert(WritePos <= Buffer.Length);
				if (WritePos == Buffer.Length)
				{
					XmlToken[] temp = new XmlToken[Buffer.Length * 2];
					Buffer.CopyTo(temp, 0);
					Buffer = temp;
				}
				Debug.Assert(WritePos < Buffer.Length);
				XmlToken.Set(ref Buffer[WritePos], nodeType, name, value);
				WritePos++;
			}

			public virtual void WriteException(Exception e)
			{
				throw e;
			}

			public virtual void Read(out XmlNodeType nodeType, out QName name, out string value)
			{
				Debug.Assert(ReadEndPos < Buffer.Length);
				XmlToken.Get(ref Buffer[ReadEndPos], out nodeType, out name, out value);
				ReadEndPos++;
			}

			public virtual void FreeTokens(int num)
			{
				ReadStartPos += num;
				ReadEndPos = ReadStartPos;
			}

			public virtual void Close()
			{
				Write(XmlNodeType.None, null, null);
			}

			public virtual void GetToken(int attNum, out QName name, out string value)
			{
				Debug.Assert(0 <= attNum && attNum < ReadEndPos - ReadStartPos - 1);
				XmlToken.Get(ref Buffer[ReadStartPos + attNum], out XmlNodeType nodeType, out name, out value);
				Debug.Assert(nodeType == (attNum == 0 ? XmlNodeType.Element : XmlNodeType.Attribute), "We use GetToken() only to access parts of start element tag.");
			}
		}

		private class TokenPipeMultiThread : TokenPipe
		{
			private Exception exception;

			public TokenPipeMultiThread(int bufferSize) : base(bufferSize) { }

			public override void Reset()
			{
				base.Reset();
				exception = null;
			}

			private void ExpandBuffer()
			{
				// Buffer is too smal for this amount of attributes.
				Debug.Assert(WritePos == ReadStartPos + Buffer.Length, "no space to write next token");
				Debug.Assert(WritePos == ReadEndPos, "all tokens ware read");
				int newMask = (Mask << 1) | 1;
				XmlToken[] newBuffer = new XmlToken[newMask + 1];
				for (int i = ReadStartPos; i < WritePos; i++)
				{
					newBuffer[i & newMask] = Buffer[i & Mask];
				}
				Buffer = newBuffer;
				Mask = newMask;
				Debug.Assert(WritePos < ReadStartPos + Buffer.Length, "we should have now space to next write token");
			}

			public override void Write(XmlNodeType nodeType, QName name, string value)
			{
				lock (this)
				{
					Debug.Assert(ReadEndPos <= WritePos && WritePos <= ReadStartPos + Buffer.Length);
					if (WritePos == ReadStartPos + Buffer.Length)
					{
						if (WritePos == ReadEndPos)
						{
							ExpandBuffer();
						}
						else
						{
							Monitor.Wait(this);
						}
					}

					Debug.Assert(WritePos < ReadStartPos + Buffer.Length);
					XmlToken.Set(ref Buffer[WritePos & Mask], nodeType, name, value);

					WritePos++;
					if (ReadStartPos + Buffer.Length <= WritePos)
					{
						// This "if" is some heuristics, it may wrk or may not:
						// To minimize task switching we wakeup reader ony if we wrote enouph tokens.
						// So if reader already waits, let it sleep before we fill up the buffer. 
						Monitor.Pulse(this);
					}
				}
			}

			public override void WriteException(Exception e)
			{
				lock (this)
				{
					exception = e;
					Monitor.Pulse(this);
				}
			}

			public override void Read(out XmlNodeType nodeType, out QName name, out string value)
			{
				lock (this)
				{
					Debug.Assert(ReadEndPos <= WritePos && WritePos <= ReadStartPos + Buffer.Length);
					if (ReadEndPos == WritePos)
					{
						if (ReadEndPos == ReadStartPos + Buffer.Length)
						{
							ExpandBuffer();
							Monitor.Pulse(this);
						}
						Monitor.Wait(this);
					}
					if (exception != null)
					{
						throw new XsltException("Exception happened during transformation. See inner exception for details:\n", exception);
					}
				}
				Debug.Assert(ReadEndPos < WritePos);
				XmlToken.Get(ref Buffer[ReadEndPos & Mask], out nodeType, out name, out value);
				ReadEndPos++;
			}

			public override void FreeTokens(int num)
			{
				lock (this)
				{
					ReadStartPos += num;
					ReadEndPos = ReadStartPos;
					Monitor.Pulse(this);
				}
			}

			public override void Close()
			{
				Write(XmlNodeType.None, null, null);
				lock (this)
				{
					Monitor.Pulse(this);
				}
			}

			public override void GetToken(int attNum, out QName name, out string value)
			{
				Debug.Assert(0 <= attNum && attNum < ReadEndPos - ReadStartPos - 1);
				XmlToken.Get(ref Buffer[(ReadStartPos + attNum) & Mask], out XmlNodeType nodeType, out name, out value);
				Debug.Assert(nodeType == (attNum == 0 ? XmlNodeType.Element : XmlNodeType.Attribute), "We use GetToken() only to access parts of start element tag.");
			}
		}
	}
}
