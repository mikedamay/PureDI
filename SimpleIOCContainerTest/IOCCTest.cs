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
            IOCC iocc = new IOCC();
            iocc.SetAssemblies("mscorlib", "System", "SimpleIOCContainerTest");
            TestRoot twf 
              = iocc.GetOrCreateObjectTree<TestRoot>();
            Assert.AreNotEqual(null, twf.test);
        }
    }

    internal class TestRoot
    {
        [IOCCInjectedDependency]
        public ITest test;
    }

    interface ITest
    {
        
    }

    [IOCCDependency]
    class Test : ITest
    {
        
    }
}
