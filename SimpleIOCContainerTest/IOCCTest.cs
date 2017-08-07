using System;
using System.Threading;
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
        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalDependencies()
        {
            // this should not run forever
            CyclicalDependency cd = IOCC.Instance.GetOrCreateObjectTree<CyclicalDependency>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().Child);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
        }
        [TestMethod]
        public void ShouldWorkWithCyclicalInterfaces()
        {
            ParentWithInterface cd 
              = IOCC.Instance.GetOrCreateObjectTree<ParentWithInterface>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.IsNotNull(cd.GetResults().IChild?.GetResults().IParent);
        }
        [TestMethod]
        public void ShouldCreateTreeForCyclicalBaseClasses()
        {
            BaseClass cd 
              = IOCC.Instance.GetOrCreateObjectTree<BaseClass>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.IsNotNull(cd?.GetResults().ChildClass?.GetResults().BasestClass);
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
