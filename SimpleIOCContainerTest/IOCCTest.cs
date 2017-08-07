using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class IOCCTest
    {
        [TestMethod]
        public void SelfTest()
        {
            IOCC iocc = new IOCC();
            iocc.SetAssemblies("mscorlib", "System", "SimpleIOCContainerTest");
            TestRoot twf 
              = iocc.GetOrCreateObjectTree<TestRoot>();
            Assert.AreNotEqual(null, twf.test);
        }
        [TestMethod]
        public void ShouldHaveRootClassWithNoArgConstructor()
        {
            void DoTest()
            {
                new IOCC().GetOrCreateObjectTree<int>();
            }
            Assert.ThrowsException<Exception>((System.Action)DoTest);
        }
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchy()
        {
            DeepHierahy root = IOCC.Instance.GetOrCreateObjectTree<DeepHierahy>();
            Assert.IsNotNull(root);
            Assert.IsNotNull(root?.GetResults().Level2a);
            Assert.IsNotNull(root?.GetResults().Level2b);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3a);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3b);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3a);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3b);
        }
        [Ignore]
        [TestMethod]
        public void ShouldWorkWithCyclicalDependencies()
        {
            Assert.Fail();
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
