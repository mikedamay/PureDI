using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class AlreadyInitialisedTest
    {
        [TestMethod]
        public void ShouldWarnIfMemberAlreadyInitialised()
        {
            // can't build using CodeAnalysis.CSharpCompilation.Emit - bad IL format
            IResultGetter result;
            IOCCDiagnostics diagnostics;
            SimpleIOCContainer sic = new SimpleIOCContainer();
            result = sic.CreateAndInjectDependencies<IOCCTest.AlreadyInitialisedTestData.AlreadyInitialised>(out diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(1, diagnostics.Groups["AlreadyInitialised"].Occurrences.Count);
            Assert.AreEqual("overwritten", result.GetResults().SomeClass.someValue);
        }
        [TestMethod]
        public void ShouldWarnIfPrimitiveAlreadyInitialised()
        {
            // can't build using CodeAnalysis.CSharpCompilation.Emit - bad IL format
            IResultGetter result;
            IOCCDiagnostics diagnostics;
            SimpleIOCContainer sic = new SimpleIOCContainer();
            result = sic.CreateAndInjectDependencies<IOCCTest.AlreadyInitialisedTestData.Primitive>(out diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(1, diagnostics.Groups["AlreadyInitialised"].Occurrences.Count);
            Assert.AreEqual(43, result.GetResults().Val);
        }
    }
}