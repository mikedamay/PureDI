using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.rootBean;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class RootObjectTest
    {
        private const string ROOT_OBJET_TEST_NAMESPACE = "RootObjectTestData";
        [TestMethod]
        public void SHouldCreateTreeForInstantiatedObject()
        {
            Simple simple = new Simple();
            SimpleIOCContainer sic = new SimpleIOCContainer(Assemblies: new[] { this.GetType().Assembly });
            //sic.SetAssemblies(typeof(RootObjectTest).Assembly.GetName().Name);
            sic.CreateAndInjectDependenciesWithObject(simple);
            Assert.IsNotNull(simple.GetResults().Child);
        }

        [TestMethod]
        public void ShouldConnectInstantiatedObjectToExistingTree()
        {
            ConnectUp connectUp = new ConnectUp();
            SimpleIOCContainer sic = new SimpleIOCContainer();
            var existing = sic.CreateAndInjectDependencies<ExistingRoot>().rootBean;
            sic.CreateAndInjectDependenciesWithObject(connectUp);
            Assert.AreEqual(connectUp.connectedChild, existing.existingChild);

        }

        [TestMethod]
        public void ShouldCreateTreeForInstantiatedObjectInHierarchy()
        {
            SimpleIOCContainer sic = new SimpleIOCContainer();
            Complex complex = sic.CreateAndInjectDependencies<Complex>().rootBean;
            Assert.AreEqual("ValueOne", complex.value1.val);
            Assert.AreEqual("ValueTwo", complex.value2.val);
        }

        [TestMethod]
        public void ShouldHookUpWithRootObjwectFromAnotherEntryPoint()
        {
            SimpleIOCContainer sic = new SimpleIOCContainer(
              Assemblies: new []{this.GetType().Assembly}, ExcludeAssemblies: SimpleIOCContainer.AssemblyExclusion.ExcludeRootTypeAssembly);
            object obj;
            sic.CreateAndInjectDependenciesWithObject((obj = new DeepHierarchy()));
            SomeUser someUser = sic.CreateAndInjectDependencies<SomeUser>().rootBean;
            Assert.IsNotNull(someUser.deep);
        }

        [TestMethod]
        public void ShouldCreateTreeForRootObjectWithoutExplictAssemblies()
        {
            SimpleIOCContainer sic = new SimpleIOCContainer();
            sic.CreateAndInjectDependenciesWithObject(new InsertedAsObject());
            InferAssembly infer = sic.CreateAndInjectDependencies<InferAssembly>().rootBean;
            Assert.IsNotNull(infer.inserted);
        }
    }
}