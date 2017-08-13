using System;
using System.Runtime.CompilerServices;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    public interface ISimpleFactory
    {
        object Execute(BeanFactoryArgs args);
    }

    [IOCCBean]
    public class SimpleFactory : ISimpleFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return null;
        }
    }
    [IOCCBean]
    public class MyBean //: IResultGetter
    {
        [IOCCBeanReference(Factory=typeof(SimpleFactory), FactoryParameter=10)]
        public int Abc;

        public object GetResults()
        {
            return null;
            //dynamic eo = new ExpandoObject();
            //eo.Abc = Abc;
            //return eo;
        }        
    }
}