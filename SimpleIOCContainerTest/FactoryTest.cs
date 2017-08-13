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
            object rootBean = iocc.GetOrCreateObjectTree("IOCCTest.FactoryTestData.MyBean", out IOCCDiagnostics diagnostics);
            Assert.IsNotNull(rootBean);
            //dynamic results = ((IResultGetter)rootBean).GetResults();
            //Assert.AreEqual(10, results.Abc);
        }


        public static string GetResource(string resourceName)
        {
            using (Stream s
                = typeof(TypeMapBuilderTest).Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}