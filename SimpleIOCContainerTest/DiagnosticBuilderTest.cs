using System;
using System.IO;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class DiagnosticBuilderTest
    {
        [TestMethod]
        public void ShouldLoadDiagnosticSchema()
        {
            string schemaName
              = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticScema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
            }
            Assert.IsTrue(true);
        }
        private static string GetResource(string resourceName)
        {
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
