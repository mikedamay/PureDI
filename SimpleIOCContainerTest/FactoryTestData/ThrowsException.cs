using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class ThrowsExceptionFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new Exception("test exception");
        }
    }
    [Bean]
    public class ThrowsException : IResultGetter
    {
        [BeanReference(Factory = typeof(ThrowsExceptionFactory))] public object someValue;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}