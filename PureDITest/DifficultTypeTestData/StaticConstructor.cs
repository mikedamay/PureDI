using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class StaticConstructor : IResultGetter
    {
        [BeanReference] private ActuallyStaticConstructor asc;

        public dynamic GetResults()
        {
            return null;
        }
    }

    [Bean]
    internal class ActuallyStaticConstructor
    {
        [Constructor]
        static ActuallyStaticConstructor()
        {
            
        }

        public ActuallyStaticConstructor(int ii)
        {
            
        }
    }
}