using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class BadFactoryFactory
    {
        
    }
    [IOCCBean]
    public class BadFactory : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(BadFactoryFactory))] private int myInt;

        public dynamic GetResults()
        {
            return new ExpandoObject();
        }
    }
}