using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Reflection;
using System.Collections.Generic;

using Mvp.Xml.Exslt;

// ReSharper disable once CheckNamespace
namespace Mvp.Xml.Common.Xsl
{
     
    /// <summary>
    /// <para><see cref="MvpXslTransform"/> class extends capabilities of the <see cref="XslCompiledTransform"/>
    /// class by adding support for transforming into <see cref="XmlReader"/>, 
    /// built-in vast collection of EXSLT extention functions, multiple outputs, XHTML output mode, 
    /// XSLT 2.0 character
    /// maps and transforming of <see cref="IXPathNavigable"/> along with <see cref="XmlResolver"/>.
    /// Also <see cref="MvpXslTransform"/> class provides new improved XSL transformation API 
    /// by introducing concepts of <see cref="IXmlTransform"/> interface, <see cref="XmlInput"/>
    /// and <see cref="XmlOutput"/>.</para>    
    /// </summary>
    /// <remarks><para><see cref="MvpXslTransform"/> class is thread-safe for Transorm() methods. I.e.
    /// once <see cref="MvpXslTransform"/> object is loaded, you can safely call its Transform() methods
    /// in multiple threads simultaneously.</para>
    /// <para><see cref="MvpXslTransform"/> supports EXSLT extension functions from the following namespaces:<br/> 
    /// * http://exslt.org/common<br/>
    /// * http://exslt.org/dates-and-times<br/>   
    /// * http://exslt.org/math<br/>
    /// * http://exslt.org/random<br/>
    /// * http://exslt.org/regular-expressions<br/>
    /// * http://exslt.org/sets<br/>
    /// * http://exslt.org/strings<br/>
    /// * http://gotdotnet.com/exslt/dates-and-times<br/>
    /// * http://gotdotnet.com/exslt/math<br/>
    /// * http://gotdotnet.com/exslt/regular-expressions<br/>
    /// * http://gotdotnet.com/exslt/sets<br/>
    /// * http://gotdotnet.com/exslt/strings<br/>
    /// * http://gotdotnet.com/exslt/dynamic</para>
    /// <para>Multioutput (&lt;exsl:document&gt; element) is turned off by default and can 
    /// be turned on using <see cref="MvpXslTransform.MultiOutput"/> property. Note, that multioutput is not supported
    /// when transfomation is done to <see cref="XmlWriter"/> or <see cref="XmlReader"/>.</para>
    /// <para><see cref="MvpXslTransform"/> uses XSLT extension objects and reflection and so using
    /// it requires FullTrust security level.</para>
    /// <para><see cref="MvpXslTransform"/> supports XSLT 2.0-like character map declarations.
    /// For character mapping semantics see <a href="http://www.w3.org/TR/xslt20/#character-maps">http://www.w3.org/TR/xslt20/#character-maps</a>.
    /// The only deviation from XSLT 2.0 is that "output", "character-map" and "output-character" elements
    /// must be in the "http://www.xmllab.net/nxslt" namespace:
    /// <pre>
    /// &lt;xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    ///     xmlns:nxslt="http://www.xmllab.net/nxslt">
    ///    &lt;nxslt:output use-character-maps="testmap"/>
    ///    &lt;nxslt:character-map name="testmap">
    ///         &lt;nxslt:output-character character="&#160;" string="&amp;nbsp;" />
    ///    &lt;/nxslt:character-map>
    /// </pre>
    /// </para>
    /// <para>XHTML output mode can be enforced by setting <see cref="EnforceXHTMLOutput"/> property.</para>
    /// <para>Author: Sergey Dubinets, Microsoft XML Team.</para>
    /// <para>Contributors: Oleg Tkachenko, <a href="http://www.xmllab.net">http://www.xmllab.net</a>.</para>
    /// </remarks>

    public class MvpXslTransform : IXmlTransform
    {
        private readonly object sync = new object();

        /// <summary>
        /// Supported EXSLT functions
        /// </summary>
        private ExsltFunctionNamespace supportedFunctions = ExsltFunctionNamespace.All;

        private Dictionary<char, string> characterMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvpXslTransform"/> class. 
        /// </summary>
        public MvpXslTransform()
        {
            CompiledTransform = new XslCompiledTransform();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvpXslTransform"/> 
        /// class with the specified debug setting. 
        /// </summary>
        public MvpXslTransform(bool debug)
        {
            CompiledTransform = new XslCompiledTransform(debug);
        }

        /// <summary>
        /// Loads and compiles the style sheet contained in the <see cref="IXPathNavigable"/> object.
        /// See also <see cref="XslCompiledTransform.Load(IXPathNavigable)"/>.
        /// </summary>
        /// <param name="stylesheet">An object implementing the <see cref="IXPathNavigable"/> interface. 
        /// In the Microsoft .NET Framework, this can be either an <see cref="XmlNode"/> (typically an <see cref="XmlDocument"/>), 
        /// or an <see cref="XPathDocument"/> containing the style sheet.</param>
        public void Load(IXPathNavigable stylesheet)
        {
            LoadStylesheetFromReader(stylesheet.CreateNavigator().ReadSubtree());            
        }

        /// <summary>
        /// Loads and compiles the style sheet located at the specified URI.
        /// See also <see cref="XslCompiledTransform.Load(String)"/>.
        /// </summary>
        /// <param name="stylesheetUri">The URI of the style sheet.</param>
        public void Load(string stylesheetUri)
        {
            LoadStylesheetFromReader(XmlReader.Create(stylesheetUri));
        }

        /// <summary>
        /// Compiles the style sheet contained in the <see cref="XmlReader"/>.
        /// See also <see cref="XslCompiledTransform.Load(XmlReader)"/>.
        /// </summary>
        /// <param name="stylesheet">An <see cref="XmlReader"/> containing the style sheet.</param>
        public void Load(XmlReader stylesheet) 
        {
            LoadStylesheetFromReader(stylesheet);
        }

        /// <summary>
        /// Compiles the XSLT style sheet contained in the <see cref="IXPathNavigable"/>. 
        /// The <see cref="XmlResolver"/> resolves any XSLT <c>import</c> or <c>include</c> elements and the 
        /// XSLT settings determine the permissions for the style sheet. 
        /// See also <see cref="XslCompiledTransform.Load(IXPathNavigable, XsltSettings, XmlResolver)"/>.
        /// </summary>                     
        /// <param name="stylesheet">An object implementing the <see cref="IXPathNavigable"/> interface. 
        /// In the Microsoft .NET Framework, this can be either an <see cref="XmlNode"/> (typically an <see cref="XmlDocument"/>), 
        /// or an <see cref="XPathDocument"/> containing the style sheet.</param>
        /// <param name="settings">The <see cref="XsltSettings"/> to apply to the style sheet. 
        /// If this is a null reference (Nothing in Visual Basic), the <see cref="XsltSettings.Default"/> setting is applied.</param>
        /// <param name="stylesheetResolver">The <see cref="XmlResolver"/> used to resolve any 
        /// style sheets referenced in XSLT <c>import</c> and <c>include</c> elements. If this is a 
        /// null reference (Nothing in Visual Basic), external resources are not resolved.</param>
        public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            LoadStylesheetFromReader(stylesheet.CreateNavigator().ReadSubtree(), settings, stylesheetResolver);
        }

        /// <summary>
        /// Loads and compiles the XSLT style sheet specified by the URI. 
        /// The <see cref="XmlResolver"/> resolves any XSLT <c>import</c> or <c>include</c> elements and the 
        /// XSLT settings determine the permissions for the style sheet. 
        /// See also <see cref="XslCompiledTransform.Load(string, XsltSettings, XmlResolver)"/>.
        /// </summary>           
        /// <param name="stylesheetUri">The URI of the style sheet.</param>
        /// <param name="settings">The <see cref="XsltSettings"/> to apply to the style sheet. 
        /// If this is a null reference (Nothing in Visual Basic), the <see cref="XsltSettings.Default"/> setting is applied.</param>
        /// <param name="stylesheetResolver">The <see cref="XmlResolver"/> used to resolve any 
        /// style sheets referenced in XSLT <c>import</c> and <c>include</c> elements. If this is a 
        /// null reference (Nothing in Visual Basic), external resources are not resolved.</param>
        public void Load(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            LoadStylesheetFromReader(XmlReader.Create(stylesheetUri), settings, stylesheetResolver);
        }

        /// <summary>
        /// Compiles the XSLT style sheet contained in the <see cref="XmlReader"/>. 
        /// The <see cref="XmlResolver"/> resolves any XSLT <c>import</c> or <c>include</c> elements and the 
        /// XSLT settings determine the permissions for the style sheet. 
        /// See also <see cref="XslCompiledTransform.Load(XmlReader, XsltSettings, XmlResolver)"/>.
        /// </summary>                
        /// <param name="stylesheet">The <see cref="XmlReader"/> containing the style sheet.</param>
        /// <param name="settings">The <see cref="XsltSettings"/> to apply to the style sheet. 
        /// If this is a null reference (Nothing in Visual Basic), the <see cref="XsltSettings.Default"/> setting is applied.</param>
        /// <param name="stylesheetResolver">The <see cref="XmlResolver"/> used to resolve any 
        /// style sheets referenced in XSLT <c>import</c> and <c>include</c> elements. If this is a 
        /// null reference (Nothing in Visual Basic), external resources are not resolved.</param>
        public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            LoadStylesheetFromReader(stylesheet, settings, stylesheetResolver);
        }

        /// <summary>
        /// Bitwise enumeration used to specify which EXSLT functions should be accessible to 
        /// the <see cref="MvpXslTransform"/> object. The default value is ExsltFunctionNamespace.All 
        /// </summary>
        public ExsltFunctionNamespace SupportedFunctions
        {
            set
            {
                if (Enum.IsDefined(typeof(ExsltFunctionNamespace), value))
                {
                    supportedFunctions = value;
                }
            }
            get => supportedFunctions;
        }

        /// <summary>
        /// Boolean flag used to specify whether multiple output (via exsl:document) is 
        /// supported.
        /// Note: This property is ignored (hence multiple output is not supported) when
        /// transformation is done to XmlReader or XmlWriter (use overloaded method, 
        /// which transforms to MultiXmlTextWriter instead).
        /// Note: Because of some restrictions and slight overhead this feature is
        /// disabled by default. If you need multiple output support, set this property to
        /// true before the Transform() call.
        /// </summary>
        public bool MultiOutput { get; set; }

        /// <summary>
        /// Enforces <see cref="MvpXslTransform"/> instance to output results in XHTML format.
        /// </summary>
        public bool EnforceXHTMLOutput { get; set; }

        /// <summary>
        /// Boolean flag used to specify whether XSLT 2.0 character maps are
        /// supported.
        /// </summary>
        /// <remarks>Note: <see cref="MvpXslTransform"/> is XSLT 1.0 processor, so 
        /// XSLT 2.0 character maps must be defined in custom "http://www.xmllab.net/nxslt"
        /// namespace. Here is a sample:
        /// &lt;xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        ///     xmlns:nxslt="http://www.xmllab.net/nxslt">
        ///     &lt;nxslt:output use-character-maps="testmap"/>
        ///     &lt;nxslt:character-map name="testmap">
        ///         &lt;nxslt:output-character character="&amp;#160;" string="&amp;amp;nbsp;" />
        ///     &lt;/nxslt:character-map>
        /// &lt;/xsl:stylesheet>
        /// </remarks>
        public bool SupportCharacterMaps { get; set; }

        ///// <summary>
        ///// Gets the TempFileCollection that contains the temporary files generated 
        ///// on disk after a successful call to the Load method. 
        ///// </summary>
        //public TempFileCollection TemporaryFiles
        //{
        //    get { return this.compiledTransform.TemporaryFiles; }
        //}

        /// <summary>
        /// Transforms given <see cref="XmlInput"/> into <see cref="XmlOutput"/>.
        /// The <see cref="XsltArgumentList"/> provides additional runtime arguments.
        /// </summary>
        /// <param name="input">Default input XML document.</param>
        /// <param name="arguments">An <see cref="XsltArgumentList"/> containing the namespace-qualified 
        /// arguments used as input to the transform. This value can be a null reference (Nothing in Visual Basic).</param>
        /// <param name="output">Represents the transformation's output.</param>
        /// <returns>Transformation output.</returns>
        public XmlOutput Transform(XmlInput input, XsltArgumentList arguments, XmlOutput output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            XmlWriter xmlWriter = output.Destination as XmlWriter;
            bool closeWriter = false;
            if (xmlWriter == null)
            {
                closeWriter = true;
                while (true)
                {
                    if (output.Destination is TextWriter txtWriter)
                    {
                        if (MultiOutput)
                        {
                            MultiXmlTextWriter mw = new MultiXmlTextWriter(txtWriter, output.XmlResolver);

                            if (CompiledTransform.OutputSettings.Indent)
                            {
                                mw.Formatting = Formatting.Indented;
                            }
                            xmlWriter = mw;
                        }
                        else
                        {
                            xmlWriter = XmlWriter.Create(txtWriter, CompiledTransform.OutputSettings);
                        }
                        break;
                    }

                    if (output.Destination is Stream strm)
                    {
                        if (MultiOutput)
                        {

                            MultiXmlTextWriter mw = new MultiXmlTextWriter(strm, CompiledTransform.OutputSettings.Encoding, output.XmlResolver);
                            if (CompiledTransform.OutputSettings.Indent)
                            {
                                mw.Formatting = Formatting.Indented;
                            }
                            xmlWriter = mw;
                        }
                        else
                        {
                            xmlWriter = XmlWriter.Create(strm, CompiledTransform.OutputSettings);
                        }
                        break;
                    }

                    if (output.Destination is string str)
                    {
                        if (MultiOutput)
                        {
                            MultiXmlTextWriter mw = new MultiXmlTextWriter(str, CompiledTransform.OutputSettings.Encoding);
                            if (CompiledTransform.OutputSettings.Indent)
                            {
                                mw.Formatting = Formatting.Indented;
                            }
                            xmlWriter = mw;
                        }
                        else
                        {
                            XmlWriterSettings outputSettings = CompiledTransform.OutputSettings.Clone();
                            outputSettings.CloseOutput = true;
                            // BugBug: We should read doc before creating output file in case they are the same
                            xmlWriter = XmlWriter.Create(str, outputSettings);
                        }
                        break;
                    }
                    throw new Exception("Unexpected XmlOutput");
                }
            }
            try
            {
                TransformToWriter(input, arguments, xmlWriter);
            }
            finally
            {
                if (closeWriter)
                {
                    xmlWriter.Close();
                }
            }
            return output;
        }

        /// <summary>
        /// Gets an <see cref="XmlWriterSettings"/> object that contains the output 
        /// information derived from the xsl:output element of the style sheet.
        /// </summary>
        public XmlWriterSettings OutputSettings => CompiledTransform != null ? CompiledTransform.OutputSettings : new XmlWriterSettings();

        /// <summary>
        /// Transforms given <see cref="XmlInput"/> into <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="input">Default input XML document</param>
        /// <param name="arguments">An <see cref="XsltArgumentList"/> containing the namespace-qualified 
        /// arguments used as input to the transform. This value can be a null reference (Nothing in Visual Basic).</param>
        public XmlReader Transform(XmlInput input, XsltArgumentList arguments)
        {
            var r = new XslReader(CompiledTransform);
            r.StartTransform(input, AddExsltExtensionObjectsSync(arguments));
            return r;
        }

        /// <summary>
        /// Transforms given <see cref="XmlInput"/> into <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="input">Default input XML document</param>
        /// <param name="arguments">An <see cref="XsltArgumentList"/> containing the namespace-qualified 
        /// arguments used as input to the transform. This value can be a null reference (Nothing in Visual Basic).</param>
        /// <param name="multiThread">Defines in which mode (multithreaded or singlethreaded)
        /// this instance of XslReader will operate</param>
        /// <param name="initialBufferSize">Initial buffer size (number of nodes, not bytes)</param>
        public XmlReader Transform(XmlInput input, XsltArgumentList arguments, bool multiThread, int initialBufferSize)
        {
            var r = new XslReader(CompiledTransform, multiThread, initialBufferSize);
            r.StartTransform(input, AddExsltExtensionObjectsSync(arguments));
            return r;
        }

        /// <summary>
        /// Core XSLT engine.
        /// </summary>
        internal XslCompiledTransform CompiledTransform { get; }

        /// <summary>
        /// Default XML Reader settings
        /// </summary>
        protected static readonly XmlReaderSettings DefaultReaderSettings;

        static MvpXslTransform()
        {
            DefaultReaderSettings = new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse};
            //DefaultReaderSettings.ProhibitDtd = false;
        }

        /// <summary>
        /// Gets XML Reader settings (customized if there is custom XML resolver)
        /// </summary>        
        /// <returns></returns>
        protected XmlReaderSettings GetReaderSettings(XmlInput defaultDocument)
        {
            if (defaultDocument.Resolver is DefaultXmlResolver)
            {
                return DefaultReaderSettings;
            }

            XmlReaderSettings settings = DefaultReaderSettings.Clone();
            settings.XmlResolver = defaultDocument.Resolver;
            return settings;
        }

        /// <summary>
        /// Transforms to XmlWriter.
        /// </summary>        
        protected void TransformToWriter(XmlInput defaultDocument, XsltArgumentList xsltArgs, XmlWriter targetWriter)
        {
            XmlWriter xmlWriter;            
            if (SupportCharacterMaps && characterMap != null && characterMap.Count > 0)
            {
                xmlWriter = new CharacterMappingXmlWriter(targetWriter, characterMap);
            }
            else
            {
                xmlWriter = targetWriter;
            }
            if (EnforceXHTMLOutput)
            {
                xmlWriter = new XhtmlWriter(xmlWriter);
            }
            XsltArgumentList args = AddExsltExtensionObjectsSync(xsltArgs);
            if (defaultDocument.Source is XmlReader xmlReader)
            {
                CompiledTransform.Transform(xmlReader, args, xmlWriter, defaultDocument.Resolver);
                return;
            }

            if (defaultDocument.Source is IXPathNavigable nav)
            {
                if (defaultDocument.Resolver is DefaultXmlResolver)
                {
                    CompiledTransform.Transform(nav, args, xmlWriter);
                }
                else
                {
                    TransformIxPathNavigable(nav, args, xmlWriter, defaultDocument.Resolver);
                }
                return;
            }

            if (defaultDocument.Source is string str)
            {
                using (XmlReader reader = XmlReader.Create(str, GetReaderSettings(defaultDocument)))
                {
                    CompiledTransform.Transform(reader, args, xmlWriter, defaultDocument.Resolver);
                }
                return;
            }

            if (defaultDocument.Source is Stream strm)
            {
                using (XmlReader reader = XmlReader.Create(strm, GetReaderSettings(defaultDocument)))
                {
                    CompiledTransform.Transform(reader, args, xmlWriter, defaultDocument.Resolver);
                }
                return;
            }

            if (defaultDocument.Source is TextReader txtReader)
            {
                using (XmlReader reader = XmlReader.Create(txtReader, GetReaderSettings(defaultDocument)))
                {
                    CompiledTransform.Transform(reader, args, xmlWriter, defaultDocument.Resolver);
                }
                return;
            }
            throw new Exception("Unexpected XmlInput");
        }

        /// <summary>
        /// Transforms to IXPathNavigable.
        /// </summary>        
        protected void TransformIxPathNavigable(IXPathNavigable nav, XsltArgumentList args, XmlWriter xmlWriter, XmlResolver resolver)
        {
            object command = CompiledTransform.GetType().GetField(
                "_command", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(CompiledTransform);
            MethodInfo executeMethod = command.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public,
                null, new[] { typeof(IXPathNavigable), typeof(XmlResolver), typeof(XsltArgumentList), typeof(XmlWriter) }, null);
            executeMethod.Invoke(command,
                new object[] { nav, resolver, AddExsltExtensionObjectsSync(args), xmlWriter });
        }

        /// <summary>
        /// Loads XSLT stylesheet from <see cref="XmlReader"/>.
        /// Wraps the reader if character maps are supported.
        /// </summary>        
        protected void LoadStylesheetFromReader(XmlReader reader)
        {
            LoadStylesheetFromReader(reader, XsltSettings.Default, new XmlUrlResolver());
        }

        /// <summary>
        /// Loads XSLT stylesheet from <see cref="XmlReader"/>, with given settings and resolver.
        /// Wraps the reader if character maps are supported.
        /// </summary>        
        protected void LoadStylesheetFromReader(XmlReader reader, XsltSettings settings, XmlResolver resolver)
        {            
            if (SupportCharacterMaps)
            {
                CharacterMappingXmlReader cmr = new CharacterMappingXmlReader(reader);
                CompiledTransform.Load(cmr, settings, resolver);
                characterMap = cmr.CompileCharacterMapping();
            }
            else
            {
                CompiledTransform.Load(reader, settings, resolver);
            }            
        }

        /// <summary>
        /// Adds the objects that implement the EXSLT extensions to the provided argument 
        /// list. The extension objects added depend on the value of the SupportedFunctions
        /// property.
        /// </summary>
        /// <param name="list">The argument list</param>
        /// <returns>An XsltArgumentList containing the contents of the list passed in 
        /// and objects that implement the EXSLT. </returns>
        /// <remarks>If null is passed in then a new XsltArgumentList is constructed. </remarks>
        protected XsltArgumentList AddExsltExtensionObjectsSync(XsltArgumentList list)
        {
            lock (sync)
            {
                list = AddExsltExtensionObjects(list, SupportedFunctions);
            }
            return list;
        }

        /// <summary>
        /// Adds the extension objects that implement all EXSLT functions to the provided 
        /// argument list.
        /// </summary>
        /// <param name="list">The argument list</param>        
        /// <returns>An XsltArgumentList containing required EXSLT extension objects. </returns>
        /// <remarks>If null is passed in then a new XsltArgumentList is constructed. </remarks>
        public static XsltArgumentList AddExsltExtensionObjects(XsltArgumentList list)
        {
            return AddExsltExtensionObjects(list, ExsltFunctionNamespace.All);
        }

        /// <summary>
        /// Adds the extension objects that implement all EXSLT functions to the provided 
        /// argument list. The extension objects added depend on the value of the supportedFunctions
        /// argument.
        /// </summary>
        /// <param name="list">The argument list</param>
        /// <param name="supportedFunctions">Required EXSLT modules</param>
        /// <returns>An XsltArgumentList containing required EXSLT extension objects. </returns>
        /// <remarks>If null is passed in then a new XsltArgumentList is constructed. </remarks>
        public static XsltArgumentList AddExsltExtensionObjects(XsltArgumentList list, ExsltFunctionNamespace supportedFunctions)
        {
            if (list == null)
            {
                list = new XsltArgumentList();
            }

            //remove all our extension objects in case the XSLT argument list is being reused                
            list.RemoveExtensionObject(ExsltNamespaces.Math);
            list.RemoveExtensionObject(ExsltNamespaces.Random);
            list.RemoveExtensionObject(ExsltNamespaces.DatesAndTimes);
            list.RemoveExtensionObject(ExsltNamespaces.RegularExpressions);
            list.RemoveExtensionObject(ExsltNamespaces.Strings);
            list.RemoveExtensionObject(ExsltNamespaces.Sets);
            list.RemoveExtensionObject(ExsltNamespaces.GdnDatesAndTimes);
            list.RemoveExtensionObject(ExsltNamespaces.GdnMath);
            list.RemoveExtensionObject(ExsltNamespaces.GdnRegularExpressions);
            list.RemoveExtensionObject(ExsltNamespaces.GdnSets);
            list.RemoveExtensionObject(ExsltNamespaces.GdnStrings);
            list.RemoveExtensionObject(ExsltNamespaces.GdnDynamic);

            //add extension objects as specified by SupportedFunctions                

            if ((supportedFunctions & ExsltFunctionNamespace.Math) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.Math, new ExsltMath());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.Random) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.Random, new ExsltRandom());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.DatesAndTimes) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.DatesAndTimes, new ExsltDatesAndTimes());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.RegularExpressions) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.RegularExpressions, new ExsltRegularExpressions());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.Strings) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.Strings, new ExsltStrings());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.Sets) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.Sets, new ExsltSets());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnDatesAndTimes) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnDatesAndTimes, new GdnDatesAndTimes());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnMath) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnMath, new GdnMath());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnRegularExpressions) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnRegularExpressions, new GdnRegularExpressions());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnSets) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnSets, new GdnSets());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnStrings) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnStrings, new GdnStrings());
            }

            if ((supportedFunctions & ExsltFunctionNamespace.GdnDynamic) > 0)
            {
                list.AddExtensionObject(ExsltNamespaces.GdnDynamic, new GdnDynamic());
            }

            return list;
        }
    }
}
