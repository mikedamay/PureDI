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
            StructRoot root = new SimpleIOCContainer().CreateAndInjectDependencies<StructRoot>().rootObject;
            Assert.IsNotNull(root.child);
        }

        [TestMethod]
        public void ShouldCreateATreeWithStructs()
        {
            StructTree tree = new SimpleIOCContainer().CreateAndInjectDependencies<StructTree>().rootObject;
            Assert.IsNotNull(tree.structChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithMixOfClassesAndStructs()
        {
            ClassTree tree = new SimpleIOCContainer().CreateAndInjectDependencies<ClassTree>().rootObject;
            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree?.GetStructChild2().GetClassChild()?.someValue);
        }
    }
}