using System.IO;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class DiagnosticBuilderTest
    {
        [TestMethod]
        public void ShouldCreateGroupFromWellFormedSchema()
        {
            string schemaName
              = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
            }
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void ShouldFailWithBadlyFormedSchema()
        {
            Assert.ThrowsException<IOCCException>(() =>
            {
                string schemaName
                  = "IOCCTest.TestData.BadDiagnosticScema.xml";
                using (Stream s
                    = typeof(IOCCTest).Assembly.GetManifestResourceStream(schemaName))
                {
                    DiagnosticBuilder db = new DiagnosticBuilder(s);
                }
            });
        }
        [TestMethod]
        public void ShouldFailWithSchemaThatContainsNoXML()
        {
            Assert.ThrowsException<IOCCException>(() =>
            {
                string schemaName
                  = "IOCCTest.TestData.SchemaWithNoXML.xml";
                using (Stream s
                    = typeof(IOCCTest).Assembly.GetManifestResourceStream(schemaName))
                {
                    DiagnosticBuilder db = new DiagnosticBuilder(s);
                }
            });
        }
        [TestMethod]
        public void ShouldSetUpInvalidBeanWarning()
        {
            IOCCDiagnostics diags;
            string schemaName
                = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
                diags = db.Diagnostics;
                dynamic diag = diags.Groups["InvalidBean"].CreateDiagnostic();
                diag.BaseClass = "testBaseClassType";
                diag.CandidateBean = "testCandidateBeanType";
                diags.Groups["InvalidBean"].Add(diag);
            }
            Assert.AreEqual("testBaseClassType"
                , ((dynamic)diags.Groups["InvalidBean"].Errors[0]).BaseClass);
            Assert.AreEqual("testCandidateBeanType"
                , ((dynamic)diags.Groups["InvalidBean"].Errors[0]).CandidateBean);
            System.Diagnostics.Debug.WriteLine(diags);
        }
        [TestMethod]
        public void ShouldReturnSubstitutionsInString()
        {
            IOCCDiagnostics diags;
            string schemaName
                = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
                diags = db.Diagnostics;
                dynamic diag = diags.Groups["InvalidBean"].CreateDiagnostic();
                diag.BaseClass = "testBaseClassType";
                diag.CandidateBean = "testCandidateBeanType";
                diags.Groups["InvalidBean"].Add(diag);
            }
            string str = diags.ToString();
            Assert.IsTrue(str.Contains("testBaseClassType"));
            Assert.IsTrue(str.Contains("testCandidateBeanType"));
        }
   }
}
