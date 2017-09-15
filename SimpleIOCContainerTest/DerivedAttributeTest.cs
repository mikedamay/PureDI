using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class DerivedAttributeTest
    {
        private const string DERIVED_ATTRIBUTE_TEST_NAMESPACE = "DerivedAttributeTestData";
        [TestMethod]
        public void ShouldCreateTreeWithDerivedConstructor()
        {
            SimpleIOCContainer sic =
                CreateIOCCinAssembly(DERIVED_ATTRIBUTE_TEST_NAMESPACE
                    , "DerivedConstructor");
            var result = sic.CreateAndInjectDependencies(
                "IOCCTest.DerivedAttributeTestData.DerivedConstructor"
                , out var diagnostics, rootConstructorName: "TestConstructor") as IResultGetter;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual("somestuff", result?.GetResults().Stuff);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

    }
}