using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class ThrowsExceptionFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new Exception("test exception");
        }
    }
    [IOCCBean]
    public class ThrowsException
    {
        [IOCCBeanReference(Factory = typeof(ThrowsExceptionFactory))] public object someValue;
    }
}