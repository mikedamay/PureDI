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
        public void ShouldCreateDeepHierarchy()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "DeepHierarchy");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(diagnostics.HasWarnings);            
        }

        [TestMethod]
        public void ShouldCreateTreeWithMultipleConstructors()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "MultipleConstructors");
            Assert.IsNotNull(result?.GetResults().Level1a?.GetResults().Level2a);
            Assert.IsNotNull(result?.GetResults().Level1b?.GetResults().Level2b);
            Assert.IsNull(result?.GetResults().Level1a?.GetResults().Level2b);
            Assert.IsNull(result?.GetResults().Level1b?.GetResults().Level2a);

            Assert.IsFalse(diagnostics.HasWarnings);            
         }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryConstructorParams()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "Factory");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeWithFactoryInjectedViaConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "FactoryConstructor");
            Assert.IsNotNull(result?.GetResults().Level1?.GetResults().Level2);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeWithPrivateConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "PrivateConstructor");
            Assert.AreEqual(42, result?.GetResults().SomeValue);
            Assert.IsFalse(diagnostics.HasWarnings);

        }
        [TestMethod]
        public void ShouldWarnIfDuplicateConstructors()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                CONSTRUCTOR_TEST_NAMESPACE, "DuplicateConstructors");
            Assert.IsTrue(diagnostics.HasWarnings);
            
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