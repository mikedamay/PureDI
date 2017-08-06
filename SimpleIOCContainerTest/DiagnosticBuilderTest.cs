using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
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
                diag.AbstractClass = "testBaseClassType";
                diags.Groups["InvalidBean"].Add(diag);
            }
            Assert.AreEqual("testBaseClassType"
                , ((dynamic)diags.Groups["InvalidBean"].Errors[0]).AbstractClass);
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
                diag.AbstractClass = "testBaseClassType";
                diags.Groups["InvalidBean"].Add(diag);
            }
            string str = diags.ToString();
            Assert.IsTrue(str.Contains("testBaseClassType"));
         }
        [TestMethod]
        public void ShouldValidateDiagnosticSchema()
        {
            bool errorsAndWarnings = false;
            void ValidateXml(XDocument xml, string xsdFilename)
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                XmlSchemaSet schemaSet = new XmlSchemaSet();

                schemaSet.Add(string.Empty, xsdFilename);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += ValidationCallback;

                xml.Validate(schemaSet, ValidationCallback);
            }

            void ValidationCallback(object sender, ValidationEventArgs args)
            {
                if (args.Severity == XmlSeverityType.Warning)
                {
                    System.Diagnostics.Debug.WriteLine($"warning: {args.Exception.Message}");
                    errorsAndWarnings = true;
                }
                else if (args.Severity == XmlSeverityType.Error)
                {
                    System.Diagnostics.Debug.WriteLine($"error: {args.Exception.Message}");
                    errorsAndWarnings = true;
                }
            }
            string schemaName
                = "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(IOCC).Assembly.GetManifestResourceStream(schemaName))
            {
                XDocument doc = XDocument.Load(s);
                ValidateXml(doc, "Docs/DiagnosticSchemaSchema.xsd");
            }
            Assert.IsFalse(errorsAndWarnings);
        }
    }
}
