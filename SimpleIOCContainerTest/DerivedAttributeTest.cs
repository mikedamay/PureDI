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

        [TestMethod]
        public void ShouldCreateTreeWithDerivedReference()
        {
            (dynamic result, var diagnostics) = CreateAndRunAssembly(
                DERIVED_ATTRIBUTE_TEST_NAMESPACE, "BeanReference");
            Assert.IsNotNull(result?.GetResults().Referred);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithDerivedBeanAndRoot()
        {
            (dynamic result, var diagnostics) = CreateAndRunAssembly(
                DERIVED_ATTRIBUTE_TEST_NAMESPACE, "Bean");
            Assert.IsNotNull(result?.GetResults().Child);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
        [TestMethod]
        public void ShouldCreateTreeWithDerivedFactory()
        {

            (dynamic result, var diagnostics) = CreateAndRunAssembly(
                DERIVED_ATTRIBUTE_TEST_NAMESPACE, "Factory");
            Assert.IsNotNull(result?.GetResults().Resource);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }
    }
}