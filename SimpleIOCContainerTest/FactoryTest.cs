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
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree("IOCCTest.FactoryTestData.AccessByString", out IOCCDiagnostics diagnostics);
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
            Assert.AreEqual(17, result?.GetResults().SomeValue );
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        private static 
          (dynamic result, IOCCDiagnostics diagnostics) 
          CommonFactoryTest(string className)
        {
            string codeText = GetResource(
                $"IOCCTest.FactoryTestData.{className}.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                $"IOCCTest.FactoryTestData.{className}"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            dynamic result = (IResultGetter) rootBean;
            return (result, diagnostics);
        }


        public static string GetResource(string resourceName)
        {
            try
            {
                using (Stream s
                    = typeof(TypeMapBuilderTest).Assembly.GetManifestResourceStream(resourceName))
                using (StreamReader sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (ArgumentNullException aue)
            {
                throw new Exception(
                  $"Most likely the file {resourceName} has not been created or has not been marked as an embedded resource in the VS project"
                  , aue);
            }
        }
    }
}