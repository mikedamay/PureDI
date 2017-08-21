using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for IOCCTest2
    /// </summary>
    [TestClass]
    public class IOCCTest2
    {
        public IOCCTest2()
        {
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
        public void ShouldCreateTreeWithReadOnlyFields()
        {
            ReadOnlyFields rof = null;
            try
            {
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependencies<ReadOnlyFields>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(rof);
            Assert.IsNotNull(rof?.GetResults().Field);
        }
        [TestMethod]
        public void ShouldCreateTreeWithAlreadyInitializedFields()
        {
            AlreadyInitialized rof = null;
            try
            {
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependencies<AlreadyInitialized>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(rof);
            Assert.IsNotNull(rof?.GetResults().Field);
        }
        [TestMethod]
        public void ShouldCreateTreeWithProperties()
        {
            MyProps props = null;
            try
            {
                props = SimpleIOCContainer.Instance.CreateAndInjectDependencies<MyProps>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(props);
            Assert.IsNotNull(props?.GetResults().MyProp);
        }
        [TestMethod]
        public void ShouldCreateTreeWithAutoProperties()
        {
            MyAutoProp props = null;
            try
            {
                props = SimpleIOCContainer.Instance.CreateAndInjectDependencies<MyAutoProp>(out IOCCDiagnostics diags);
                Assert.IsTrue(diags.HasWarnings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldThrowExceptionForNoArgConstructor()
        {   
            Assert.ThrowsException<IOCCException>(() =>
            {
                NoArgRoot st = new SimpleIOCContainer().CreateAndInjectDependencies<
                  NoArgRoot>(out IOCCDiagnostics diags);
                Assert.IsTrue(diags.HasWarnings);
            });
        }
        [TestMethod]
        public void ShouldThrowExceptionForNoArgClassTree()
        {
            NoArgClassTree nact = new SimpleIOCContainer().CreateAndInjectDependencies<
                NoArgClassTree>(out IOCCDiagnostics diags);
            Assert.IsTrue(diags.HasWarnings);
        }

        [TestMethod]
        public void ShouldInstantiateSingleObjectFromMultipleInterfaces()
        {
            ClassWithMultipleInterfaces cwmi 
              = new SimpleIOCContainer().CreateAndInjectDependencies<ClassWithMultipleInterfaces>();
            Assert.IsNotNull(cwmi?.GetResults().Interface1);
            Assert.IsTrue(cwmi?.GetResults().Interface1 == cwmi?.GetResults().Interface2);
        }

        [TestMethod]
        public void ShouldInstantiateStruct()
        {
            StructRoot root = new SimpleIOCContainer().CreateAndInjectDependencies<StructRoot>();
            Assert.IsNotNull(root.child);
        }

        [TestMethod]
        public void ShouldCreateATreeWithStructs()
        {
            StructTree tree = new SimpleIOCContainer().CreateAndInjectDependencies<StructTree>();
            Assert.IsNotNull(tree.structChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithMixOfClassesAndStructs()
        {
            ClassTree tree = new SimpleIOCContainer().CreateAndInjectDependencies<ClassTree>();
            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree?.GetStructChild2().GetClassChild()?.someValue);
        }

        [TestMethod]
        public void ShouldCreateASingleInstanceForMultipleReferences()
        {
            CrossConnections cc = new SimpleIOCContainer().CreateAndInjectDependencies<CrossConnections>();
            Assert.IsNotNull(cc?.ChildA?.Common);
            Assert.IsTrue(cc?.ChildA?.Common == cc?.ChildB?.Common);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenerics()
        {
            RefersToGeneric rtg = new SimpleIOCContainer().CreateAndInjectDependencies<RefersToGeneric>();
            Assert.AreEqual("Generic<T>", rtg?.GenericInt?.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericRoot()
        {
            Generic<int> gi = new SimpleIOCContainer().CreateAndInjectDependencies<Generic<int>>();
            Assert.IsNotNull(gi);
        }
        [TestMethod]
        public void ShouldCreateTreeWithGenericParameter()
        {
            GenericHolderParent ghp = new SimpleIOCContainer().CreateAndInjectDependencies<GenericHolderParent>();
            Assert.AreEqual("GenericHeld", ghp?.GenericHolder?.GenericHeld.Name);
        }
        [TestMethod]
        public void ShouldCreateTreeWhenRootHasGenericParameter()
        {
            GenericHolder<GenericHeld> ghgh 
              = new SimpleIOCContainer().CreateAndInjectDependencies<GenericHolder<GenericHeld>>();
            Assert.AreEqual("GenericHeld", ghgh?.GenericHeld?.Name);
        }
    }
}
