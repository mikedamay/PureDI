using System;
using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class GenericFactoryX : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (new MyGeneric<int>(), injectionState);
        }
    }
    [Bean]
    public class Generic : IResultGetter
    {
        [BeanReference(Factory=typeof(GenericFactoryX))]
        private MyGeneric<int> myGeneric;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyGeneric = myGeneric;
            return eo;
        }
    }
    internal class MyGeneric<T>
    {
    }
}