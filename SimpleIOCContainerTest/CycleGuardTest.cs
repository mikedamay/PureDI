using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for CycleGuardTest
    /// </summary>
    [TestClass]
    public class CycleGuardTest
    {
        [TestMethod]
        public void ShouldBuildTreeWithSelfReferentialClass()
        {
            SelfReferring sr = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<SelfReferring>();
            Assert.IsNotNull(sr);
        }
        [TestMethod, Timeout(1000)]
        public void ShouldWorkWithCyclicalDependencies()
        {
            try
            {
                // this should not run forever
                CyclicalDependency cd = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<CyclicalDependency>();
                Assert.IsNotNull(cd);
                Assert.IsNotNull(cd?.GetResults().Child);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().Parent);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild);
                Assert.IsNotNull(cd?.GetResults().Child?.GetResults().GrandChild?.GetResults().GrandParent);
            }
            catch (StackOverflowException)
            {
                Assert.Fail("The stack overflowed indicating cyclical dependencies");
            }
        }
        [TestMethod, Timeout(1000)]
        public void ShouldWorkWithCyclicalInterfaces()
        {
            ParentWithInterface cd
                = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<ParentWithInterface>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.IsNotNull(cd.GetResults().IChild?.GetResults().IParent);
        }
        [TestMethod, Timeout(1000)]
        public void ShouldCreateTreeForCyclicalBaseClasses()
        {
            BaseClass cd
                = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<BaseClass>();
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.IsNotNull(cd?.GetResults().ChildClass?.GetResults().BasestClass);
        }
        [TestMethod, Timeout(1000)]
        public void ShouldWorkWithCyclicalInterfacesWithNames()
        {
            TestCode.WithNames.ParentWithInterface cd
                = SimpleIOCContainer.Instance.CreateAndInjectDependencies<TestCode.WithNames.ParentWithInterface>(out IOCCDiagnostics diags, rootBeanName: "name-B");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd.GetResults().IChild);
            Assert.AreEqual("name-B", cd.GetResults().IChild?.GetResults().IParent?.GetResults().Name);
            Assert.AreEqual("name-B2", cd.GetResults().IChild?.GetResults().IParent2?.GetResults().Name);
        }
        [TestMethod, Timeout(1000)]
        public void ShouldCreateTreeForCyclicalBaseClassesWithNames()
        {
            TestCode.WithNames.BaseClass cd
                = SimpleIOCContainer.Instance.CreateAndInjectDependencies<TestCode.WithNames.BaseClass>(out IOCCDiagnostics diags, rootBeanName: "basest");
            Assert.IsNotNull(cd);
            Assert.IsNotNull(cd?.GetResults().ChildClass);
            Assert.AreEqual("basest", cd?.GetResults().ChildClass?.GetResults().BasestClass?.GetResults().Name);
        }
    
        public CycleGuardTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        CycleGuard cycleGuard;
        [TestInitialize]
        public void Setup()
        {
            cycleGuard = new CycleGuard();
        }
        [TestMethod]
        public void SimpleCycleGuardTest()
        {
            Assert.IsFalse(cycleGuard.IsPresent(typeof(string)));
            cycleGuard.Push(typeof(string));
            Assert.IsTrue(cycleGuard.IsPresent(typeof(string)));
            cycleGuard.Pop();
            Assert.IsFalse(cycleGuard.IsPresent(typeof(string)));
        }
        [TestMethod]
        public void ConstructedGenericCycleGuardTest()
        {
            Assert.IsFalse(cycleGuard.IsPresent(typeof(List<int>)));
            cycleGuard.Push(typeof(List<int>));
            Assert.IsTrue(cycleGuard.IsPresent(typeof(List<int>)));
            cycleGuard.Pop();
            Assert.IsFalse(cycleGuard.IsPresent(typeof(List<int>)));
        }

        [TestMethod]
        public void ShouldNotConfuseConstructedGenerics()
        {
            cycleGuard.Push(typeof(List<int>));
            Assert.IsFalse(cycleGuard.IsPresent(typeof(List<string>)));
            Assert.IsTrue(cycleGuard.IsPresent(typeof(List<int>)));
            cycleGuard.Pop();
            Assert.IsFalse(cycleGuard.IsPresent(typeof(List<int>)));
        }
    }

}
