using System;
using System.Diagnostics;

using Mvp.Xml.XInclude;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif


namespace Mvp.Xml.XInclude.Test
{
    /// <summary>
    /// XInclude syntax tests.
    /// </summary>
    [TestClass]
    public class XIncludeSyntaxTests
    {        

        public XIncludeSyntaxTests() 
        {
            //Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
        }

        /// <summary>
        /// No href and no xpointer attribute.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void NoHrefAndNoXPointerAttributes() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/nohref.xml");
            while (xir.Read());
            xir.Close();
        }        

        /// <summary>
        /// xi:include child of xi:include.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void IncludeChildOfInclude() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/includechildofinclude.xml");
            while (xir.Read());
            xir.Close();
        }
                
        /// <summary>
        /// xi:fallback not child of xi:include.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void FallbackNotChildOfInclude() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/fallbacknotchildinclude.xml");
            while (xir.Read());
            xir.Close();
        }     
       
        /// <summary>
        /// Unknown value of parse attribute.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void UnknownParseAttribute() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/unknownparseattr.xml");
            while (xir.Read());
            xir.Close();
        }     

        /// <summary>
        /// Two xi:fallback.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(XIncludeSyntaxError))]
        public void TwoFallbacks() 
        {
			XIncludingReader xir = new XIncludingReader("../../../XInclude/tests/twofallbacks.xml");
            while (xir.Read());
            xir.Close();
        }             
    }
}
