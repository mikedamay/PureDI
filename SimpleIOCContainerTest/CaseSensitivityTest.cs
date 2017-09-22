using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;
namespace IOCCTest
{
    [TestClass]
    public class CaseSensitivityTest
    {
        [TestMethod]
        public void ShouldCreateTreeForTheCorrectUpperCasedClass()
        {
            (var result, var diagnostics)
                = CreateAndRunAssembly("CaseSensitivityTestData", "Simple");
            Assert.AreEqual("Simple", result?.GetResults().Val);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeForTheCorrectLowerCasedClass()
        {
            (var result, var diagnostics)
                = CreateAndRunAssembly("CaseSensitivityTestData", "simple2");
            Assert.AreEqual("simple2", result?.GetResults().Val);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouleCreateTreeWithAMixOfCases()
        {
            (var result, var diagnostics)
                = CreateAndRunAssembly("CaseSensitivityTestData", "Hierarchy");
            Assert.AreEqual("lowercase", result?.GetResults().LowerCase);
            Assert.AreEqual("uppercase", result?.GetResults().UpperCase);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouleCreateTreeWithInterfaceImplementedByMixedCases()
        {
            (var result, var diagnostics)
                = CreateAndRunAssembly("CaseSensitivityTestData", "Interface");
            Assert.AreEqual("lowercase", result?.GetResults().LowerCase);
            Assert.AreEqual("uppercase", result?.GetResults().UpperCase);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
    }
}