using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for CycleGuardTest
    /// </summary>
    [TestClass]
    public class CycleGuardTest
    {
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
        public void ShouldAddAndRemoveDependency()
        {
            Assert.IsFalse(cycleGuard.IsCyclicalDependency(typeof(string)));
            cycleGuard.AddCyclicalDependency(typeof(string));
            Assert.IsTrue(cycleGuard.IsCyclicalDependency(typeof(string)));
            cycleGuard.RemoveCyclicalDependency(typeof(string));
            Assert.IsFalse(cycleGuard.IsCyclicalDependency(typeof(string)));
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
