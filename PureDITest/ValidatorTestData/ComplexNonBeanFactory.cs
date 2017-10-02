using System;
using PureDI;

namespace IOCCTest.DifficultTypeTestData
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class SomeNonFactoryAttribute : Attribute
    {
        
    }
    public class ComplexNonBeanFactory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
    [Bean]
    public class AWellFornedFactory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }

    [SomeNonFactory]
    [SomeNonFactory]
    public class ANonFatory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new NotImplementedException();
        }
    }
    [SomeNonFactory]
    [SomeNonFactory]
    public class ANonFatory2 : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new NotImplementedException();
        }
    }


}