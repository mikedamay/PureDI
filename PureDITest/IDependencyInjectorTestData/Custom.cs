using System;
using System.Reflection;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.IDependencyInjectorTestData
{
    [Bean]
    public class Custom
    {
        [BeanReference(Factory = typeof(CustomFactory))]
        public Child child;
    }
    [Bean]
    public class Child
    {
        
    }
    [Bean]
    public class CustomFactory : IFactory
    {
        [BeanReference] private IDependencyInjector injector = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<Child>(injectionState);
        }
    }

    public static class Creator
    {
        static Creator()
        {
                
        }
    }
    
    //[Bean]
    public class CustomInjector : IDependencyInjector
    {
        private DependencyInjector stdInjector;

        public CustomInjector(string[] profiles, bool ignoreRootTypeAssembly)
        {
            stdInjector = new DependencyInjector(profiles, ignoreRootTypeAssembly);
        }

        public (TRootType rootBean, InjectionState injectionState) CreateAndInjectDependencies<TRootType>(
            InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            return stdInjector.CreateAndInjectDependencies<TRootType>(injectionState, assemblies, rootBeanSpec);
        }

        public (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(Type rootType,
            InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            return stdInjector.CreateAndInjectDependencies(rootType, injectionState, assemblies, rootBeanSpec);
        }

        public (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(string rootTypeName,
            InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            return stdInjector.CreateAndInjectDependencies(rootTypeName, injectionState, assemblies, rootBeanSpec);
        }

        public (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(object rootObject,
            InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null,
            bool deferDepedencyInjection = false)
        {
            return stdInjector.CreateAndInjectDependencies(rootObject
              ,injectionState, assemblies, rootBeanSpec, deferDepedencyInjection);
        }

        public (TBean, InjectionState) CreateBean<TBean>(InjectionState injectionState = null) where TBean : class
        {
            return CreateBean<TBean>(injectionState);
        }
    }
}