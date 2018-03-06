using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class SimpleFactory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (args.FactoryParmeter, injectionState);
        }
    }
    [Bean]
    public class SimpleBean : IResultGetter
    {
        [BeanReference(Factory=typeof(SimpleFactory), FactoryParameter=10)]
        public int Abc;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Abc = Abc;
            return eo;
        }        
    }
}