using System.Dynamic;
using System.Linq.Expressions;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class GenericFactoryFactory<T> : IFactory where T : new()
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (new T(), injectionState);
        }
    }
    // this is not really a factory
    [Bean]
    public class GenericFactory : IResultGetter
    {
        [BeanReference(Factory = typeof(GenericFactoryFactory<MyThing>))]
        private MyThing myThing;
        [BeanReference(Factory = typeof(GenericFactoryFactory<MySecondThing>))]
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