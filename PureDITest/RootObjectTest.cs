using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
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
            (_, @is) = pdi.CreateAndInjectDependencies(new AlreadyInstantiated(), injectionState: @is
              );
            Assert.AreEqual(1, @is.Diagnostics.Groups["RootObjectExists"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldNotWarnIfPrototypeRootObjectWasAlreadyInjected()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            (_, @is) = pdi.CreateAndInjectDependencies<AlreadyInstantiated>();
            (_, @is) = pdi.CreateAndInjectDependencies(new AlreadyInstantiated(), injectionState: @is
              ,rootBeanSpec: new RootBeanSpec(scope: BeanScope.Prototype));
            Assert.AreEqual(0, @is.Diagnostics.Groups["RootObjectExists"].Occurrences.Count);
        }
        [TestMethod]    // prototype not deferred
        public void ShouldCreateDependenciesForPrototypeRootObject()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            var rootBean = new AlreadyInstantiated();
            (_, @is) = pdi.CreateAndInjectDependencies(rootBean
              ,rootBeanSpec: new RootBeanSpec(scope: BeanScope.Prototype));
            Assert.IsNotNull(rootBean?.Child);
        }
        [TestMethod]    // prototype deferred
        public void ShouldNotWarnOfPrototypeRootObjectWithDeferredFlag()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            var rootBean = new AlreadyInstantiated();
            (_, @is) = pdi.CreateAndInjectDependencies(rootBean
              ,rootBeanSpec: new RootBeanSpec(scope: BeanScope.Prototype), deferDepedencyInjection: true);
                    // pointless but we don't warn.
            Assert.IsNull(rootBean?.Child);
        }
        [TestMethod]    // singleton not deferred
        public void ShouldRespectNotDeferredFlag()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            var rootBean = new AlreadyInstantiated();
            (_, @is) = pdi.CreateAndInjectDependencies(rootBean
              ,deferDepedencyInjection: false);
            Assert.IsNotNull(rootBean?.Child);
        }
        [TestMethod]    // singleton deferred
        public void ShouldRespectDeferredFlag()
        {
            var pdi = new PDependencyInjector();
            InjectionState @is;
            var rootBean = new AlreadyInstantiated();
            (_, @is) = pdi.CreateAndInjectDependencies(rootBean
              ,deferDepedencyInjection: true);
            Assert.IsNull(rootBean?.Child);
            (_, @is) = pdi.CreateAndInjectDependencies<AlreadyInstantiated>(@is);
            Assert.IsNotNull(rootBean?.Child);
        }

        [TestMethod]
        public void ShouldRespectConstructorNameOnRootObject()
        {
            var pdi = new PDependencyInjector();
            var named = new NamedConstructor(new MyParam());
            InjectionState @is;
            (_, @is) = pdi.CreateAndInjectDependencies(named,
                rootBeanSpec: new RootBeanSpec(rootConstructorName: "MyConstructor"));
            (var other, var is2) = pdi.CreateAndInjectDependencies<AnotherClass>(@is);
            Assert.IsNotNull(other.named);
        }

        [TestMethod]
        public void ShouldInjectDependenciesForRootObjectBaseClass()
        {
            var pdi = new PDependencyInjector();
            var @base = new BaseClasses();
            InjectionState @is;
            (_, @is) = pdi.CreateAndInjectDependencies(@base);
            (SomeClassUser scu, InjectionState is2) = pdi.CreateAndInjectDependencies<SomeClassUser>(@is);
            Assert.IsNotNull(scu.BaseClasses);
            Assert.IsNotNull(scu.SomeBaseClass);
            Assert.IsNotNull(scu.SomeInterface);

        }

        [TestMethod]
        public void ShouldCreateNonBeanWithCreateBeanMethod()
        {
            var pdi = new PDependencyInjector();
            (var nonBean, var @is) = pdi.CreateBean<NonBean>();
            Assert.IsNotNull(nonBean);
        }

        [TestMethod]
        public void ShouldCreateBeanWithCreateBeanMethod()
        {
            try
            {
                var pdi = new PDependencyInjector();
                (var unnamedBean, var @is) = pdi.CreateBean<UnnamedBean>();
                Assert.IsNotNull(unnamedBean);
                (var _, var is2) = pdi.CreateAndInjectDependencies(unnamedBean, @is
                    ,rootBeanSpec: new RootBeanSpec(rootBeanName: "BeanWithAName")
                );
                (var bean, var is3) = pdi.CreateAndInjectDependencies<Bean>(is2);
                Assert.IsNotNull(bean?.RefToUnnamedBean);
            }
            catch (DIException dix)
            {
                Console.WriteLine(dix.Diagnostics.AllToString());
                Assert.Fail();
            }
        }
    }
}