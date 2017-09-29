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
            SimpleIOCContainer iocc 
              = new SimpleIOCContainer(Assemblies: new[] { this.GetType().Assembly });
            //iocc.SetAssemblies("mscorlib", "System", "SimpleIOCContainerTest");
            TestRoot twf 
              = iocc.CreateAndInjectDependencies<TestRoot>().rootBean;
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
            DeepHierahy root = SimpleIOCContainer.Instance.CreateAndInjectDependencies<global::IOCCTest.TestCode.DeepHierahy>().rootBean;
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
            WithNames.DeepHierahy root = SimpleIOCContainer.Instance.CreateAndInjectDependencies<WithNames.DeepHierahy>().rootBean;
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
                    WithNames.CyclicalDependency>(rootBeanName: "name-A").rootBean;
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().Child);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
            Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
        }
        [TestMethod]
        public void ShouldCreateASingleInstanceForMultipleReferences()
        {
            CrossConnections cc = new SimpleIOCContainer().CreateAndInjectDependencies<CrossConnections>().rootBean;
            Assert.IsNotNull(cc?.ChildA?.Common);
            Assert.IsTrue(cc?.ChildA?.Common == cc?.ChildB?.Common);
        }

        [TestMethod]
        public void ShouldCreateTreeForInheritedBeanWithDifferentName()
        {
            (WithNames.InheritedBean ib, InjectionState InjectionState)
                = SimpleIOCContainer.Instance.CreateAndInjectDependencies<
                    WithNames.InheritedBean>();
            Assert.IsNotNull(ib.derived);
            Assert.IsFalse(InjectionState.Diagnostics.ToString().Contains(typeof(WithNames.InheritedBean).Name));
        }
        [TestMethod]
        public void ShouldCreateTreeForInheritedBeanWithDifferentProfile()
        {
            (WithNames.BeanUser bu, InjectionState InjectionState)
                = SimpleIOCContainer.Instance.CreateAndInjectDependencies<
                    WithNames.BeanUser>();
            IOCCDiagnostics diags = InjectionState.Diagnostics;
            Assert.AreEqual("Inherited", bu.Used.Val);
            Assert.IsFalse(diags.ToString().Contains(typeof(WithNames.InheritedBeanWithProfile).Name));
            (bu, InjectionState)
                = new SimpleIOCContainer(Profiles: new[] { "some-profile" }).CreateAndInjectDependencies<
                    WithNames.BeanUser>();
            diags = InjectionState.Diagnostics;
            Assert.AreEqual("Derived", bu.Used.Val);
            Assert.IsFalse(diags.ToString().Contains(typeof(WithNames.InheritedBeanWithProfile).Name));
        }
        [TestMethod]
        public void ShouldCreateTreeForDerivedBeanIfInheritedIsIgnored()
        {
            (WithNames.IgnoredBeanUser bu, InjectionState InjectionState)
                = SimpleIOCContainer.Instance.CreateAndInjectDependencies<
                    WithNames.IgnoredBeanUser>();
            IOCCDiagnostics diags = InjectionState.Diagnostics;
            Assert.AreEqual("Inherited and ignored", bu.InheritedIgnorer.Val);
            Assert.AreEqual("Derived from ignorer", bu.DerivedFromIgnorer.Val);
            Assert.IsFalse(diags.ToString().Contains(typeof(WithNames.InheritedBeanWithProfile).Name));
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
