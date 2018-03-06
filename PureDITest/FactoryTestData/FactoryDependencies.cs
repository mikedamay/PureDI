using System.Dynamic;
using System.Runtime.CompilerServices;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class FactoryDependenciesFactory : IFactory
    {
        [BeanReference]
        private NumberProvider numberProvider;
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (numberProvider.number, injectionState);
        }
    }
    [Bean]
    public class NumberProvider
    {
        public int number = 17;
    }

    [Bean]
    public class FactoryDependencies : IResultGetter
    {
        [BeanReference(Factory = typeof(FactoryDependenciesFactory))] private int someValue;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = someValue;
            return eo;
        }
    }
}