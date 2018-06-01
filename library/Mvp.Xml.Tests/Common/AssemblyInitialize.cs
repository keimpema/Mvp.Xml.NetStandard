using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mvp.Xml.Tests.Common
{
    [TestClass]
    public class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
