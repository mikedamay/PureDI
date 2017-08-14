using System;
using System.Dynamic;
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
    public class ThrowsException : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(ThrowsExceptionFactory))] public object someValue;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}