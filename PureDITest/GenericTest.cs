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
            RefersToGeneric rtg = new PDependencyInjector().CreateAndInjectDependencies<RefersToGeneric>().rootBean;
            Assert.AreEqual("Generic<T>", rtg?.GenericInt?.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericRoot()
        {
            Generic<int> gi = new PDependencyInjector().CreateAndInjectDependencies<Generic<int>>().rootBean;
            Assert.IsNotNull(gi);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericParameter()
        {
            GenericHolderParent ghp = new PDependencyInjector().CreateAndInjectDependencies<GenericHolderParent>().rootBean;
            Assert.AreEqual("GenericHeld", ghp?.GenericHolder?.GenericHeld.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWhenRootHasGenericParameter()
        {
            GenericHolder<GenericHeld> ghgh
                = new PDependencyInjector().CreateAndInjectDependencies<GenericHolder<GenericHeld>>().rootBean;
            Assert.AreEqual("GenericHeld", ghgh?.GenericHeld?.Name);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericsWithMultipleParameters()
        {
            MultipleParamGenericUser mpgu
                = new PDependencyInjector().CreateAndInjectDependencies<MultipleParamGenericUser>().rootBean;
            Assert.IsNotNull(mpgu?.Multiple);
        }

        [TestMethod]
        public void ShouldCreateTreeForNestedGeneric()
        {
            WrapperUser wu = new PDependencyInjector().CreateAndInjectDependencies<WrapperUser>().rootBean;
            Assert.IsNotNull(wu?.Nested);
        }
    }
}