using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using PureDI;
using PureDI.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class DiagnosticBuilderTest
    {
        private static readonly string ResourceLocationPrefix = Common.ResourcePrefix;
        // .NET framework uses namespace, .NET core uses assembly name
        // "PureDI"
        private static readonly string TestResourcePrefix = ResourceLocationPrefix;
        [TestMethod]
        public void ShouldCreateGroupFromWellFormedSchema()
        {
            string schemaName
              = $"{ResourceLocationPrefix}.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
            }
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void ShouldFailWithBadlyFormedSchema()
        {
            Assert.ThrowsException<IOException>(() =>
            {
                string schemaName
                  = $"{TestResourcePrefix}.TestData.BadDiagnosticScema.xml";
                using (Stream s
                    = typeof(HierarchyTest).Assembly.GetManifestResourceStream(schemaName))
                {
                    DiagnosticBuilder db = new DiagnosticBuilder(s);
                }
            });
        }
        [TestMethod]
        public void ShouldFailWithSchemaThatContainsNoXML()
        {
            Assert.ThrowsException<IOException>(() =>
            {
                string schemaName
                  = $"{TestResourcePrefix}.TestData.SchemaWithNoXML.badxml";
                using (Stream s
                    = typeof(HierarchyTest).Assembly.GetManifestResourceStream(schemaName))
                {
                    DiagnosticBuilder db = new DiagnosticBuilder(s);
                }
            });
        }
        [TestMethod]
        public void ShouldSetUpInvalidBeanWarning()
        {
            Diagnostics diags;
            string schemaName
                = $"{ResourceLocationPrefix}.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
                diags = db.Diagnostics;
                dynamic diag = diags.Groups["InvalidBean"].CreateDiagnostic();
                diag.AbstractOrStaticClass = "testBaseClassType";
                diag.ClassMode = "MyClassMode";
                diags.Groups["InvalidBean"].Add(diag);
            }
            Assert.AreEqual("testBaseClassType"
                , ((dynamic)diags.Groups["InvalidBean"].Occurrences[0]).AbstractOrStaticClass);
        }
        [TestMethod]
        public void ShouldReturnSubstitutionsInString()
        {
            Diagnostics diags;
            string schemaName
                = $"{ResourceLocationPrefix}.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
                diags = db.Diagnostics;
                dynamic diag = diags.Groups["InvalidBean"].CreateDiagnostic();
                diag.AbstractOrStaticClass = "testBaseClassType";
                diag.ClassMode = "MyClassMode";
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

            string schemaSchemaName = "DiagnosticSchemaSchema.xsd";
            string schemaName
                = $"{ResourceLocationPrefix}.Docs.DiagnosticSchema.xml";
            string schemaSchemaResourcePath
                = $"{ResourceLocationPrefix}.Docs.{schemaSchemaName}";
            using (Stream @in
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaSchemaResourcePath))
            using (Stream @out = File.Open(schemaSchemaName, FileMode.Create))
            {
                byte[] bytes = new byte[1024];
                int bytesRead;
                while ((bytesRead = @in.Read(bytes, 0, bytes.Length)) > 0)
                {
                    @out.Write(bytes, 0, bytesRead);
                }
            }
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                XDocument doc = XDocument.Load(s);
                ValidateXml(doc, schemaSchemaName);
            }
            Assert.IsFalse(errorsAndWarnings);
        }
    }
}
