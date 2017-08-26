using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class ScopeTest
    {
        [TestMethod]
        public void TestAssemblyMaker()
        {
            new AssemblyMaker().MakeAssembly("class mike {}");
    

        }
        [TestMethod]
        public void shouldBuildTreeWithSimplePrototype()
        {
            (var result, var diagnostics) = CommonScopeTest("SimpleScope");
            Assert.IsNotNull(result?.GetResults().MemberA);
            Assert.AreNotEqual(result?.GetResults().MemberA, result?.GetResults().MemberB);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
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
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
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
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));

        }

        [TestMethod]
        public void ShouldCreateRootAsPrototype()
        {
            string className = "RootPrototype";
            var iocc = MakeIOCCForTestAssembly(className);
            object rootBean = iocc.CreateAndInjectDependencies(
                $"IOCCTest.ScopeTestData.{className}"
                , out IOCCDiagnostics diagnostics1, scope: BeanScope.Prototype);
            object rootBean2 = iocc.CreateAndInjectDependencies(
                $"IOCCTest.ScopeTestData.{className}"
                , out IOCCDiagnostics diagnostics2, scope: BeanScope.Prototype);
            System.Diagnostics.Debug.WriteLine(diagnostics1);
            System.Diagnostics.Debug.WriteLine(diagnostics2);
            Assert.AreNotEqual(rootBean, rootBean2);
            Assert.IsFalse(Falsify(diagnostics1.HasWarnings));

        }
        private static
            (dynamic result, IOCCDiagnostics diagnostics)
            CommonScopeTest(string className)
        {
            return Utils.CreateAndRunAssembly($"ScopeTestData", className);
        }

        private static SimpleIOCContainer MakeIOCCForTestAssembly(string className)
        {
            return Utils.CreateIOCCinAssembly($"ScopeTestData", className);
        }
    }
}