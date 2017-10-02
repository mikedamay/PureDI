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
            PDependencyInjector sic = new PDependencyInjector(Assemblies: new[] { this.GetType().Assembly });
            //sic.SetAssemblies(typeof(RootObjectTest).Assembly.GetName().Name);
            sic.CreateAndInjectDependencies(simple);
            Assert.IsNotNull(simple.GetResults().Child);
        }

        [TestMethod]
        public void ShouldConnectInstantiatedObjectToExistingTree()
        {
            ConnectUp connectUp = new ConnectUp();
            PDependencyInjector sic = new PDependencyInjector();
            var existing = sic.CreateAndInjectDependencies<ExistingRoot>();
            sic.CreateAndInjectDependencies( connectUp, existing.injectionState);
            Assert.AreEqual(connectUp.connectedChild, existing.rootBean.existingChild);

        }

        [TestMethod]
        public void ShouldCreateTreeForInstantiatedObjectInHierarchy()
        {
            PDependencyInjector sic = new PDependencyInjector();
            Complex complex = sic.CreateAndInjectDependencies<Complex>().rootBean;
            Assert.AreEqual("ValueOne", complex.value1.val);
            Assert.AreEqual("ValueTwo", complex.value2.val);
        }

        [TestMethod]
        public void ShouldHookUpWithRootObjwectFromAnotherEntryPoint()
        {
            PDependencyInjector sic = new PDependencyInjector(
              Assemblies: new []{this.GetType().Assembly}, ExcludeAssemblies: PDependencyInjector.AssemblyExclusion.ExcludeRootTypeAssembly);
            object obj;
            InjectionState injectionState;
            (obj, injectionState) = sic.CreateAndInjectDependencies(new DeepHierarchy());
            SomeUser someUser = sic.CreateAndInjectDependencies<SomeUser>(injectionState).rootBean;
            Assert.IsNotNull(someUser.deep);
        }

        [TestMethod]
        public void ShouldCreateTreeForRootObjectWithoutExplictAssemblies()
        {
            PDependencyInjector sic = new PDependencyInjector();
            (object inserted, InjectionState InjectionState) 
              = sic.CreateAndInjectDependencies(new InsertedAsObject());
            InferAssembly infer = sic.CreateAndInjectDependencies<InferAssembly>(InjectionState).rootBean;
            Assert.IsNotNull(infer.inserted);
        }
    }
}