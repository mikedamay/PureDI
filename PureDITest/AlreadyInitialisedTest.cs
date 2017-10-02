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
            InjectionState injectionState;
            PDependencyInjector pdi = new PDependencyInjector();
            (result, injectionState) = pdi.CreateAndInjectDependencies<IOCCTest.AlreadyInitialisedTestData.AlreadyInitialised>();
            var diagnostics = injectionState.Diagnostics;
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
            InjectionState injectionState;
            PDependencyInjector pdi = new PDependencyInjector();
            (result, injectionState) = pdi.CreateAndInjectDependencies<IOCCTest.AlreadyInitialisedTestData.Primitive>();
            var diagnostics = injectionState.Diagnostics;
            Assert.IsTrue(diagnostics.HasWarnings);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(1, diagnostics.Groups["AlreadyInitialised"].Occurrences.Count);
            Assert.AreEqual(43, result.GetResults().Val);
        }
    }
}