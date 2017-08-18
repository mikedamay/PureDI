using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class ConstructorTest
    {
        private const string CONSTRUCTOR_TEST_NAMESPACE = "ConstructorTestData";
        [TestMethod]
        public void ShouldCreateTreeWithSimpleConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "SimpleConstructor");
            Assert.AreEqual(42, result?.GetResults().SomeValue);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeWithMultipleParameters()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "MultipleParams");
            Assert.IsNotNull(result?.GetResults().ParamOne);
            Assert.IsNotNull(result?.GetResults().ParamTwo);
            Assert.IsFalse(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldCreateDeepHierarchyWithConstructorParams()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "DeepHierarchy");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(diagnostics.HasWarnings);            
        }

        [TestMethod]
        public void ShouldCreateTreeWithMultipleConstructors()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryConstructorParams()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryInjectedViaConstructor()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldWarnIfDuplicateConstructors()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldWarnIfSomeParametersAreNotMarkedForInjetion()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldWarnIfParmeterNotInjectable()
        {
            Assert.Fail();
        }
        [TestMethod]
        public void ShouldWarnIfCyclicalDependency()
        {
            Assert.Fail();
        }


    }
}