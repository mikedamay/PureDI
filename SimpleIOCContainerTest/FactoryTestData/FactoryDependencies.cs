using System.Dynamic;
using System.Runtime.CompilerServices;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class FactoryDependenciesFactory : IOCCFactory
    {
        [IOCCBeanReference]
        private NumberProvider numberProvider;
        public object Execute(BeanFactoryArgs args)
        {
            return numberProvider.number;
        }
    }
    [IOCCBean]
    public class NumberProvider
    {
        public int number = 17;
    }

    [IOCCBean]
    public class FactoryDependencies : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(FactoryDependenciesFactory))] private int someValue;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = someValue;
            return eo;
        }
    }
}