using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class GenericTest
    {
        [TestMethod]
        public void ShouldCreateTreeWithGenerics()
        {
            RefersToGeneric rtg = new SimpleIOCContainer().CreateAndInjectDependencies<RefersToGeneric>();
            Assert.AreEqual("Generic<T>", rtg?.GenericInt?.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericRoot()
        {
            Generic<int> gi = new SimpleIOCContainer().CreateAndInjectDependencies<Generic<int>>();
            Assert.IsNotNull(gi);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericParameter()
        {
            GenericHolderParent ghp = new SimpleIOCContainer().CreateAndInjectDependencies<GenericHolderParent>();
            Assert.AreEqual("GenericHeld", ghp?.GenericHolder?.GenericHeld.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWhenRootHasGenericParameter()
        {
            GenericHolder<GenericHeld> ghgh
                = new SimpleIOCContainer().CreateAndInjectDependencies<GenericHolder<GenericHeld>>();
            Assert.AreEqual("GenericHeld", ghgh?.GenericHeld?.Name);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericsWithMultipleParameters()
        {
            MultipleParamGenericUser mpgu
                = new SimpleIOCContainer().CreateAndInjectDependencies<MultipleParamGenericUser>();
            Assert.IsNotNull(mpgu?.Multiple);
        }

        [TestMethod]
        public void ShouldCreateTreeForNestedGeneric()
        {
            WrapperUser wu = new SimpleIOCContainer().CreateAndInjectDependencies<WrapperUser>();
            Assert.IsNotNull(wu?.Nested);
        }
    }
}