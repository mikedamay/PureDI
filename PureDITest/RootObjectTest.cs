using System.Reflection;
using PureDI;
using PureDI.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDITest.RootObjectTestCode;

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
            PDependencyInjector pdi = new PDependencyInjector();
            pdi.CreateAndInjectDependencies(simple, assemblies: new Assembly[] { this.GetType().Assembly});
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
            PDependencyInjector pdi = new PDependencyInjector();
            object obj;
            InjectionState injectionState;
            (obj, injectionState) = pdi.CreateAndInjectDependencies(new DeepHierarchy());
            SomeUser someUser = pdi.CreateAndInjectDependencies<SomeUser>(injectionState
              ,assemblies: new Assembly[] { this.GetType().Assembly}
              ).rootBean;
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

        [TestMethod]
        public void ShouldCreateUniqueInstancesWithNamedRootObject()
        {
            Instance instanceInstance = new Instance();
            var pdi = new PDependencyInjector();
            InjectionState @is, is2;
            (_, @is) = pdi.CreateAndInjectDependencies(instanceInstance, rootBeanSpec:new RootBeanSpec("MyInstance"));
            MultipleInstances multiple;
            (multiple, is2) = pdi.CreateAndInjectDependencies<MultipleInstances>(@is);
            Assert.IsNotNull(multiple.InstanceInstance);
            Assert.IsNotNull(multiple.ClassInstance);
            Assert.AreNotEqual(multiple.ClassInstance, multiple.InstanceInstance);
        }

        [TestMethod]
        public void ShouldWarnIfRootObjectWasAlreadyInjected()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            (_, @is) = pdi.CreateAndInjectDependencies<AlreadyInstantiated>();
            (_, @is) = pdi.CreateAndInjectDependencies(new AlreadyInstantiated(), injectionState: @is);
            Assert.AreEqual(1, @is.Diagnostics.Groups["RootObjectExists"].Occurrences.Count);
        }
    }
}