using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class BadFactoryFactory
    {
        
    }
    [Bean]
    public class BadFactory : IResultGetter
    {
        [BeanReference(Factory = typeof(BadFactoryFactory))] private int myInt;

        public dynamic GetResults()
        {
            return new ExpandoObject();
        }
    }
}