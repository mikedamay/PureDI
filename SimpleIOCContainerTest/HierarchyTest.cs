using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WithNames = IOCCTest.TestCode.WithNames;

namespace IOCCTest
{
    [TestClass]
    public class HierarchyTest
    {
        [TestMethod]
        public void SelfTest()
        {
            SimpleIOCContainer iocc = new SimpleIOCContainer();
            iocc.SetAssemblies("mscorlib", "System", "SimpleIOCContainerTest");
            TestRoot twf 
              = iocc.CreateAndInjectDependencies<TestRoot>();
            Assert.IsNotNull(twf.test);
        }
        [TestMethod]
        public void ShouldHaveRootClassWithNoArgConstructor()
        {
            void DoTest()
            {
                new SimpleIOCContainer().CreateAndInjectDependencies<int>();
            }
            Assert.ThrowsException<IOCCException>((System.Action)DoTest);
        }
        enum Mike { Mike1}
        [TestMethod]
        public void ShouldNotTreatEnumAsClass()
        {
            void DoTest()
            {
                
                new SimpleIOCContainer().CreateAndInjectDependencies<Mike>();
            }
            Assert.ThrowsException<IOCCException>((System.Action)DoTest);
        }
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchy()
        {
            DeepHierahy root = SimpleIOCContainer.Instance.CreateAndInjectDependencies<DeepHierahy>();
            Assert.IsNotNull(root);
            Assert.IsNotNull(root?.GetResults().Level2a);
            Assert.IsNotNull(root?.GetResults().Level2b);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3a);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3b);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3a);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3b);
        }
       
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchyWithNames()
        {
            WithNames.DeepHierahy root = SimpleIOCContainer.Instance.CreateAndInjectDependencies<WithNames.DeepHierahy>();
            Assert.IsNotNull(root);
            Assert.IsNotNull(root?.GetResults().Level2a);
            Assert.IsNotNull(root?.GetResults().Level2b);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3a);
            Assert.IsNotNull(root?.GetResults().Level2a?.GetResults().Level2a3b);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3a);
            Assert.IsNotNull(root?.GetResults().Level2b?.GetResults().Level2b3b);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForBeansWithNames()
        {
            WithNames.CyclicalDependency cd 
              = SimpleIOCContainer.Instance.CreateAndInjectDependencies<
                    WithNames.CyclicalDependency>(out IOCCDiagnostics diags, SimpleIOCContainer.DEFAULT_PROFILE, "name-A");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().Child);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
        }
        [TestMethod]
        public void ShouldCreateASingleInstanceForMultipleReferences()
        {
            CrossConnections cc = new SimpleIOCContainer().CreateAndInjectDependencies<CrossConnections>();
            Assert.IsNotNull(cc?.ChildA?.Common);
            Assert.IsTrue(cc?.ChildA?.Common == cc?.ChildB?.Common);
        }
    }
    [Bean]
    internal class TestRoot
    {
#pragma warning disable 649
        [BeanReference]
        public ITest test;
    }

    interface ITest
    {
        
    }

    [Bean]
    class Test : ITest
    {
        
    }
}
