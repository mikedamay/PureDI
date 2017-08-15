using System.Dynamic;
using System.Linq.Expressions;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class GenericFactoryFactory<T> : IOCCFactory where T : new()
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new T();
        }
    }
    // this is not really a factory
    [IOCCBean]
    public class GenericFactory : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(GenericFactoryFactory<MyThing>))]
        private MyThing myThing;
        [IOCCBeanReference(Factory = typeof(GenericFactoryFactory<MySecondThing>))]
        private MySecondThing mySecondThing;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyThing = myThing;
            eo.MySecondThing = mySecondThing;
            return eo;
        }
    }

    public class MyThing
    {
    }

    public class MySecondThing
    {
        
    }

}