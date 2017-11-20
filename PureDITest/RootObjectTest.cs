using PureDI;
using IOCCTest.rootBean;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI.Public;
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
            PDependencyInjector pdi = new PDependencyInjector(assemblies: new[] { this.GetType().Assembly });
            //pdi.SetAssemblies(typeof(RootObjectTest).Assembly.GetName().Name);
            pdi.CreateAndInjectDependencies(simple);
            Assert.IsNotNull(simple.GetResults().Child);
        }

        [TestMethod]
        public void ShouldConnectInstantiatedObjectToExistingTree()
        {
            ConnectUp connectUp = new ConnectUp();
            PDependencyInjector pdi = new PDependencyInjector();
            var existing = pdi.CreateAndInjectDependencies<ExistingRoot>();
            pdi.CreateAndInjectDependencies( connectUp, existing.injectionState);
            Assert.AreEqual(connectUp.connectedChild, existing.rootBean.existingChild);

        }

        [TestMethod]
        public void ShouldCreateTreeForInstantiatedObjectInHierarchy()
        {
            PDependencyInjector pdi = new PDependencyInjector();
            Complex complex = pdi.CreateAndInjectDependencies<Complex>().rootBean;
            Assert.AreEqual("ValueOne", complex.value1.val);
            Assert.AreEqual("ValueTwo", complex.value2.val);
        }

        [TestMethod]
        public void ShouldHookUpWithRootObjwectFromAnotherEntryPoint()
        {
            PDependencyInjector pdi = new PDependencyInjector(
              assemblies: new []{this.GetType().Assembly}, excludeAssemblies: AssemblyExclusion.ExcludeRootTypeAssembly);
            object obj;
            InjectionState injectionState;
            (obj, injectionState) = pdi.CreateAndInjectDependencies(new DeepHierarchy());
            SomeUser someUser = pdi.CreateAndInjectDependencies<SomeUser>(injectionState).rootBean;
            Assert.IsNotNull(someUser.deep);
        }

        [TestMethod]
        public void ShouldCreateTreeForRootObjectWithoutExplictAssemblies()
        {
            PDependencyInjector pdi = new PDependencyInjector();
            (object inserted, InjectionState InjectionState) 
              = pdi.CreateAndInjectDependencies(new InsertedAsObject());
            InferAssembly infer = pdi.CreateAndInjectDependencies<InferAssembly>(InjectionState).rootBean;
            Assert.IsNotNull(infer.inserted);
        }
    }
}