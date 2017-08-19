using System;
using System.IO;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void ShouldCreateTreeFromRootAsString()
        {

            string codeText = GetResource("IOCCTest.FactoryTestData.AccessByString.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            SimpleIOCContainer iocc = new SimpleIOCContainer();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree("IOCCTest.FactoryTestData.AccessByString",
                out IOCCDiagnostics diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldBuildTreeWithSimpleFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("SimpleBean");
            Assert.AreEqual(10, result.Abc);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldBuildTreeWithFactoryAndMemberBeans()
        {
            (var result, var diagnostics) = CommonFactoryTest("FactoryWithMemberBeans");
            Assert.IsNotNull(result?.GetResults().Member);
            Assert.IsNotNull(result?.GetResults().Member?.GetResults().SubMember);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldWarnIfFactoryMissing()
        {
            (var result, var diagnostics) = CommonFactoryTest("MissingFactory");
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual(1, diagnostics.Groups["MissingFactory"].Occurrences.Count);
        }

        [TestMethod]
        public void ShouldCreateTreeForFactoryWithGenerics()
        {
            (var result, var diagnostics) = CommonFactoryTest("Generic");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result?.GetResults().MyGeneric);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldWarnOnFactoryMismatch()
        {
            (var result, var diagnostics) = CommonFactoryTest("TypeMismatch");
            Assert.AreEqual(1, diagnostics.Groups["TypeMismatch"].Occurrences.Count);
        }

        [TestMethod]
        public void ShouldWarnIfFactoryExeccuteThrowsExcption()
        {
            (var result, var diagnostics) = CommonFactoryTest("ThrowsException");
            Assert.AreEqual(1, diagnostics.Groups["FactoryExecutionFailure"].Occurrences.Count);

        }

        [TestMethod]
        public void ShouldCreateTreeForFactoryWithDependencies()
        {
            (var result, var diagnostics) = CommonFactoryTest("FactoryDependencies");
            Assert.AreEqual(17, result?.GetResults().SomeValue);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("GenericFactory");
            Assert.IsNotNull(result?.GetResults().MyThing);
            Assert.IsNotNull(result?.GetResults().MySecondThing);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldWarnOfBadFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("BadFactory");
            Assert.IsTrue(diagnostics.HasWarnings);
        }

        private static
            (dynamic result, IOCCDiagnostics diagnostics)
            CommonFactoryTest(string className)
        {
            return Utils.CreateAndRunAssembly("FactoryTestData", className);
        }

        public static string GetResource(string resourceName)
        {
            return Utils.GetResource(resourceName);
        }
    }
}