using System;
using System.Text;
using System.Collections.Generic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class IOCCTest3
    {
        public IOCCTest3()
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

        [TestMethod]
        public void ShouldWarnIfTypeMissing()
        {
            MissingType mt 
              = new SimpleIOCContainer().GetOrCreateObjectTree<MissingType>(
              out IOCCDiagnostics diags);
            Assert.IsTrue(diags.HasWarnings);
            dynamic diagnostic = diags.Groups["MissingBean"]?.Occurrences[0];
            Assert.IsNotNull(diagnostic);
            Assert.AreEqual("IOCCTest.TestCode.MissingType", diagnostic?.Bean);
            Assert.AreEqual("System.Int32", diagnostic.MemberType);
            Assert.AreEqual("ii", diagnostic.MemberName);
            Assert.AreEqual("", diagnostic.MemberBeanName);
        }

        [TestMethod]
        public void ShouldErrorIfMissingRootType()
        {
            IOCCDiagnostics diags = null;
            Assert.ThrowsException<IOCCException>(() =>
                new SimpleIOCContainer().GetOrCreateObjectTree<int>(out diags));
            Assert.IsTrue(diags.HasWarnings);
            dynamic diagnostic = diags.Groups["MissingRoot"]?.Occurrences[0];
            Assert.AreEqual("System.Int32", diagnostic.BeanType);
            Assert.AreEqual("", diagnostic.BeanName);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericsWithMultipleParameters()
        {
            MultipleParamGenericUser mpgu 
              = new SimpleIOCContainer().GetOrCreateObjectTree<MultipleParamGenericUser>();
            Assert.IsNotNull(mpgu?.Multiple);
        }

        [TestMethod]
        public void ShouldCreateTreeForNestedGeneric()
        {
            WrapperUser wu = new SimpleIOCContainer().GetOrCreateObjectTree<WrapperUser>();
            Assert.IsNotNull(wu?.Nested);
        }
    }
}
