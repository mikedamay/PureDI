using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class StructTest
    {
        [TestMethod]
        public void ShouldInstantiateStruct()
        {
            StructRoot root = new SimpleIOCContainer().CreateAndInjectDependenciesSimple<StructRoot>();
            Assert.IsNotNull(root.child);
        }

        [TestMethod]
        public void ShouldCreateATreeWithStructs()
        {
            StructTree tree = new SimpleIOCContainer().CreateAndInjectDependenciesSimple<StructTree>();
            Assert.IsNotNull(tree.structChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithMixOfClassesAndStructs()
        {
            ClassTree tree = new SimpleIOCContainer().CreateAndInjectDependenciesSimple<ClassTree>();
            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree?.GetStructChild2().GetClassChild()?.someValue);
        }
    }
}