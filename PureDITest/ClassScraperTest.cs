using System.IO;
using IOCCTest.TestCode;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI;
using PureDI.Tree;

namespace IOCCTest
{
    [TestClass]
    public class ClassScraperTest
    {
        [TestMethod]
        public void ShouldFetchNoParamsFprEmptyConstructor()
        {
            var cs = new ClassScraper();
            var constructorParams = cs.CreateConstructorTrees(
              typeof(IOCCTest.ClassScraperTestCode.EmptyConstructor)
              ,PureDI.Common.Constants.DefaultConstructorName
              ,createDiagnostics()
              );
            Assert.AreEqual(0, constructorParams.Count);
        }

        private Diagnostics createDiagnostics()
        {
            string schemaName
                = $"{PureDI.Common.Common.ResourcePrefix}.Docs.DiagnosticSchema.xml";
            using (Stream s
                = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(schemaName))
            {
                DiagnosticBuilder db = new DiagnosticBuilder(s);
                return db.Diagnostics;
            }
        }
    }
}