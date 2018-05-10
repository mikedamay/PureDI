using System;
using System.IO;
using System.Linq;
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
        public void ShouldFetchNoParamsForEmptyConstructor()
        {
            var cs = new ClassScraper();
            var constructorParams = cs.GetConstructorParameterBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.EmptyConstructor)
              ,PureDI.Common.Constants.DefaultConstructorName
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(0, constructorParams.Count);
        }

        [TestMethod]
        public void ShouldFetchNoParamsForDefaultConstructor()
        {
            var cs = new ClassScraper();
            var constructorParams = cs.GetConstructorParameterBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.DefaultConstructor)
              ,PureDI.Common.Constants.DefaultConstructorName
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(0, constructorParams.Count);
        }
        
        [TestMethod]
        public void ShouldFetchParamsForConstructorWithArgs()
        {
            var cs = new ClassScraper();
            var constructorParams = cs.GetConstructorParameterBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.Vanilla)
              ,PureDI.Common.Constants.DefaultConstructorName
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(1, constructorParams.Count);
        }
        
        [TestMethod]
        public void ShouldFetchParamsForNamedConstructorWithArgs()
        {
            var cs = new ClassScraper();
            var constructorParams = cs.GetConstructorParameterBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.NamedConstructor)
              ,"MyConstructor"
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(1, constructorParams.Count);
        }
        
        [TestMethod]
        public void ShouldFetchNoMembersForEmptyClass()
        {
            var cs = new ClassScraper();
            var members = cs.GetMemberBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.NamedConstructor)
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(0, members.Count);
        }
        
        [TestMethod]
        public void ShouldFetchMembersForClassWithMembers()
        {
            var cs = new ClassScraper();
            var members = cs.GetMemberBeanReferences(
              typeof(IOCCTest.ClassScraperTestCode.Vanilla)
              ,createDiagnostics()
              ).ToList();
            Assert.AreEqual(2, members.Count);
        }
        
        [TestMethod]
        public void ShouldThrowExceptionIfConstructorHasMissingParameters()
        {
            try
            {
                var cs = new ClassScraper();
                var members = cs.GetConstructorParameterBeanReferences(
                    typeof(IOCCTest.ClassScraperTestCode.NoConstructor)
                    ,PureDI.Common.Constants.DefaultConstructorName
                    ,createDiagnostics()
                ).ToList();
                Assert.Fail();
            }
            catch (DIException e)
            {
                Assert.IsTrue(e.Diagnostics.ToString().Contains("MissingConstructorParameterAttribute"));
            }
        }
        
        [TestMethod]
        public void ShouldThrowExceptionIfDuplicateConstructors()
        {
            try
            {
                var cs = new ClassScraper();
                var members = cs.GetConstructorParameterBeanReferences(
                    typeof(IOCCTest.ClassScraperTestCode.DuplicateConstructors)
                    ,PureDI.Common.Constants.DefaultConstructorName
                    ,createDiagnostics()
                ).ToList();
                Assert.Fail();
            }
            catch (DIException e)
            {
                Assert.IsTrue(e.Diagnostics.ToString().Contains("DuplicateConstructors"));
            }
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