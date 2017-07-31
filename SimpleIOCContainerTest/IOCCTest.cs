using System;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class IOCCTest
    {
        [TestMethod]
        public void RootClassShouldHaveNoArgConstructor()
        {
            void DoTest()
            {
                IOCC.Instance.GetOrCreateObjectTree<int>();
            }
            Assert.ThrowsException<Exception>((System.Action)DoTest);
        }

        [TestMethod]
        public void ShouldBuildTreeFromWellFormedFields()
        {
            TestIOCC twf 
              = IOCC.Instance.GetOrCreateObjectTree<TestIOCC>();
            Assert.AreNotEqual(null, twf.childOne);
        }
    }
}
