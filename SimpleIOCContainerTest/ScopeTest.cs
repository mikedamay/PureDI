using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class ScopeTest
    {
        [TestMethod]
        public void shouldBuildTreeWithSimplePrototype()
        {
            (var result, var diagnostics) = CommonScopeTest("SimpleScope");
            Assert.IsNotNull(result?.GetResults().MemberA);
            Assert.AreNotEqual(result?.GetResults().MemberA, result?.GetResults().MemberB);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void shouldBuildTreeWithPrototypesWithSingleton()
        {
            (var result, var diagnostics) = CommonScopeTest("PrototypesWithSingletons");
            Assert.IsNotNull(result?.GetResults().MemberA);
            Assert.AreNotEqual(result?.GetResults().MemberA, result?.GetResults().MemberB);
            Assert.IsNotNull((result?.GetResults().MemberA as IResultGetter)?.GetResults().MemberA);
            Assert.AreEqual((result?.GetResults().MemberA as IResultGetter)?.GetResults().MemberA
               , (result?.GetResults().MemberB as IResultGetter)?.GetResults().MemberA);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        /// <summary>
        /// the factory should be the prototype
        /// </summary>
        [TestMethod]
        public void ShoudApplyPrototypeScopeToFactoryNotReferencedMember()
        {
            (var result, var diagnostics) = CommonScopeTest("FactoryPrototype");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result?.GetResults().FirstNumber);
            Assert.AreEqual(1, result?.GetResults().SecondNumber);
            Assert.IsFalse(diagnostics.HasWarnings);

        }

        [TestMethod]
        public void ShouldCreateRootAsPrototype()
        {
            string className = "RootPrototype";
            var iocc = MakeIOCCForTestAssembly(className);
            object rootBean = iocc.GetOrCreateObjectTree(
                $"IOCCTest.ScopeTestData.{className}"
                , out IOCCDiagnostics diagnostics1, scope: BeanScope.Prototype);
            object rootBean2 = iocc.GetOrCreateObjectTree(
                $"IOCCTest.ScopeTestData.{className}"
                , out IOCCDiagnostics diagnostics2, scope: BeanScope.Prototype);
            System.Diagnostics.Debug.WriteLine(diagnostics1);
            System.Diagnostics.Debug.WriteLine(diagnostics2);
            Assert.AreNotEqual(rootBean, rootBean2);
            Assert.IsFalse(diagnostics1.HasWarnings);
            Assert.IsFalse(diagnostics2.HasWarnings);

        }
        private static
            (dynamic result, IOCCDiagnostics diagnostics)
            CommonScopeTest(string className)
        {
            var iocc = MakeIOCCForTestAssembly(className);
            object rootBean = iocc.GetOrCreateObjectTree(
                $"IOCCTest.ScopeTestData.{className}"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            dynamic result = (IResultGetter)rootBean;
            return (result, diagnostics);
        }

        private static IOCC MakeIOCCForTestAssembly(string className)
        {
            string codeText = FactoryTest.GetResource(
                $"IOCCTest.ScopeTestData.{className}.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            return iocc;
        }
    }
}