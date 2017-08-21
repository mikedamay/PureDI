using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class GenericFactoryX : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new MyGeneric<int>();
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