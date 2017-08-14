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
            string codeText = GetResource("IOCCTest.FactoryTestData.SimpleFactory.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
              "IOCCTest.FactoryTestData.MyBean"
              , out IOCCDiagnostics diagnostics);
            Assert.IsNotNull(rootBean);
            dynamic results = ((IResultGetter)rootBean).GetResults();
            Assert.AreEqual(10, results.Abc);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldBuildTreeWithFactoryAndMemberBeans()
        {
            string codeText = GetResource("IOCCTest.FactoryTestData.FactoryWithMemberBeans.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.FactoryWithMemberBeans"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            dynamic results = ((IResultGetter)rootBean).GetResults();
            Assert.IsNotNull(results?.Member);
            Assert.IsNotNull(results?.Member?.GetResults().SubMember);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldWarnIfFactoryMissing()
        {
            string codeText = GetResource(
              "IOCCTest.FactoryTestData.MissingFactory.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.MissingFactory"
                , out IOCCDiagnostics diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual(1, diagnostics.Groups["MissingFactory"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldCreateTreeForFactoryWithGenerics()
        {
            string codeText = GetResource(
                "IOCCTest.FactoryTestData.Generic.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.Generic"
                , out IOCCDiagnostics diagnostics);
            dynamic results = (IResultGetter)rootBean;
            Assert.IsNotNull(results);
            Assert.IsNotNull(results?.GetResults().MyGeneric);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldWarnOnFactoryMismatch()
        {
            string codeText = GetResource(
                "IOCCTest.FactoryTestData.TypeMismatch.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.TypeMismatch"
                , out IOCCDiagnostics diagnostics);
            Assert.AreEqual(1, diagnostics.Groups["TypeMismatch"].Occurrences.Count);
        }

        [TestMethod]
        public void ShouldWarnIfFactoryExeccuteThrowsExcption()
        {
            string codeText = GetResource(
                "IOCCTest.FactoryTestData.ThrowsException.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.ThrowsException"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(1, diagnostics.Groups["FactoryExecutionFailure"].Occurrences.Count);

        }
        [TestMethod]
        public void ShouldCreateTreeForFactoryWithDependencies()
        {
            string codeText = GetResource(
                "IOCCTest.FactoryTestData.FactoryDependencies.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree(
                "IOCCTest.FactoryTestData.FactoryDependencies"
                , out IOCCDiagnostics diagnostics);
            dynamic result = (IResultGetter) rootBean;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(17, result?.GetResults().SomeValue );
            Assert.IsFalse(diagnostics.HasWarnings);
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