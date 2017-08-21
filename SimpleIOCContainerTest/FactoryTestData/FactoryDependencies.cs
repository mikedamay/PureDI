using System.Dynamic;
using System.Runtime.CompilerServices;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class FactoryDependenciesFactory : IFactory
    {
        [BeanReference]
        private NumberProvider numberProvider;
        public object Execute(BeanFactoryArgs args)
        {
            return numberProvider.number;
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