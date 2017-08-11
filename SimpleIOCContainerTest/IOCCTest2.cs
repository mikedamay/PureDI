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
                rof = IOCC.Instance.GetOrCreateObjectTree<ReadOnlyFields>();
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
                rof = IOCC.Instance.GetOrCreateObjectTree<AlreadyInitialized>();
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
                props = IOCC.Instance.GetOrCreateObjectTree<MyProps>();
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
                props = IOCC.Instance.GetOrCreateObjectTree<MyAutoProp>(out IOCCDiagnostics diags);
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
                NoArgRoot st = new IOCC().GetOrCreateObjectTree<
                  NoArgRoot>(out IOCCDiagnostics diags);
                Assert.IsTrue(diags.HasWarnings);
            });
        }
        [TestMethod]
        public void ShouldThrowExceptionForNoArgClassTree()
        {
            NoArgClassTree nact = new IOCC().GetOrCreateObjectTree<
                NoArgClassTree>(out IOCCDiagnostics diags);
            Assert.IsTrue(diags.HasWarnings);
        }

        [TestMethod]
        public void ShouldInstantiateSingleObjectFromMultipleInterfaces()
        {
            ClassWithMultipleInterfaces cwmi 
              = new IOCC().GetOrCreateObjectTree<ClassWithMultipleInterfaces>();
            Assert.IsNotNull(cwmi?.GetResults().Interface1);
            Assert.IsTrue(cwmi?.GetResults().Interface1 == cwmi?.GetResults().Interface2);
        }

        [TestMethod]
        public void ShouldInstantiateStruct()
        {
            StructRoot root = new IOCC().GetOrCreateObjectTree<StructRoot>();
            Assert.IsNotNull(root.child);
        }

        [TestMethod]
        public void ShouldCreateATreeWithStructs()
        {
            StructTree tree = new IOCC().GetOrCreateObjectTree<StructTree>();
            Assert.IsNotNull(tree.structChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithMixOfClassesAndStructs()
        {
            ClassTree tree = new IOCC().GetOrCreateObjectTree<ClassTree>();
            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree?.GetStructChild2().GetClassChild()?.someValue);
        }

        [TestMethod]
        public void ShouldCreateASingleInstanceForMultipleReferences()
        {
            CrossConnections cc = new IOCC().GetOrCreateObjectTree<CrossConnections>();
            Assert.IsNotNull(cc?.ChildA?.Common);
            Assert.IsTrue(cc?.ChildA?.Common == cc?.ChildB?.Common);
        }
        [Ignore]
        [TestMethod]
        public void ShouldCreateTreeWithGenerics()
        {
            RefersToGeneric rtg = new IOCC().GetOrCreateObjectTree<RefersToGeneric>();
            Assert.AreEqual("Generic<T>", rtg?.GenericInt?.Name);
        }
        [Ignore]
        [TestMethod]
        public void ShouldCreateTreeWithGenericRoot()
        {
            Generic<int> gi = new IOCC().GetOrCreateObjectTree<Generic<int>>();
            Assert.IsNotNull(gi);
        }
        [Ignore]
        [TestMethod]
        public void ShouldCreateTreeWithGenericParameter()
        {
            GenericHolderParent ghp = new IOCC().GetOrCreateObjectTree<GenericHolderParent>();
            Assert.AreEqual("GenericHolder", ghp?.GenericHolder?.GenericHeld.Name);
        }
        [Ignore]
        [TestMethod]
        public void ShouldCreateTreeWhenRootHasGenericParameter()
        {
            GenericHolder<GenericHeld> ghgh 
              = new IOCC().GetOrCreateObjectTree<GenericHolder<GenericHeld>>();
            Assert.AreEqual("GenericHeld", ghgh?.GenericHeld?.Name);
        }
    }
}
