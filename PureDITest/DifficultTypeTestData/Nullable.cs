using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class Nullable : IResultGetter
    {
        [BeanReference]
        int? abc;

        public dynamic GetResults()
        {
            return null;
        }
    }
}