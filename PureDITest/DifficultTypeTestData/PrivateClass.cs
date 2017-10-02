using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class PrivateClass : IResultGetter
    {
        [Bean]
        private class InnerPrivate
        {
            
        }

        [BeanReference] private InnerPrivate inner;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Inner = inner;
            return eo;
        }
    }
}