using System.Collections.Generic;
using System.IO;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for FactoryTest
    /// </summary>
    [TestClass]
    public class FactoryTest
    {
        public FactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeFromString()
        {
            //string codeText = GetResource("IOCCTest.FactoryTestData.AccessByString.cs");
            string codeText = GetResource("IOCCTest.TestData.AbstractClass.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCC iocc = new IOCC();
            iocc.SetAssemblies(assembly.GetName().Name);
            object rootBean = iocc.GetOrCreateObjectTree("AccessByString", out IOCCDiagnostics diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsFalse(diagnostics.HasWarnings);
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
