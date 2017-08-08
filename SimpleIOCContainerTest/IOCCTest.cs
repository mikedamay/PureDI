using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WithNames = IOCCTest.TestCode.WithNames;

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
        [TestMethod]
        public void ShouldBuildTreeWithSelfReferentialClass()
        {
            SelfReferring sr = IOCC.Instance.GetOrCreateObjectTree<SelfReferring>();
            Assert.IsNotNull(sr);
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
        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalInterfaces()
        {
            ParentWithInterface cd 
              = IOCC.Instance.GetOrCreateObjectTree<ParentWithInterface>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.IsNotNull(cd.GetResults().IChild?.GetResults().IParent);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForCyclicalBaseClasses()
        {
            BaseClass cd 
              = IOCC.Instance.GetOrCreateObjectTree<BaseClass>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.IsNotNull(cd?.GetResults().ChildClass?.GetResults().BasestClass);
        }
        [TestMethod]
        public void ShouldInjectIntoDeepHierarchyWithNames()
        {
            WithNames.DeepHierahy root = IOCC.Instance.GetOrCreateObjectTree<WithNames.DeepHierahy>();
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
              = IOCC.Instance.GetOrCreateObjectTree<
                    WithNames.CyclicalDependency>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().Child);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
        }

        [TestMethod, Timeout(100)]
        public void ShouldWorkWithCyclicalInterfacesWithNames()
        {
            WithNames.ParentWithInterface cd
                = IOCC.Instance.GetOrCreateObjectTree<WithNames.ParentWithInterface>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.AreEqual("name-B", cd.GetResults().IChild?.GetResults().IParent?.GetResults().Name);
            Assert.AreEqual("name-B2", cd.GetResults().IChild?.GetResults().IParent2?.GetResults().Name);
        }
        [TestMethod, Timeout(100)]
        public void ShouldCreateTreeForCyclicalBaseClassesWithNames()
        {
            WithNames.BaseClass cd
                = IOCC.Instance.GetOrCreateObjectTree<WithNames.BaseClass>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.AreEqual("basest", cd?.GetResults().ChildClass?.GetResults().BasestClass?.GetResults().Name);
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
