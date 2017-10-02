using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class TypeMismatchFactory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (42, injectionState);
        }
    }
    [Bean]
    public class TypeMismatch : IResultGetter
    {
        [BeanReference(Factory = typeof(TypeMismatchFactory))] public string memberString;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}