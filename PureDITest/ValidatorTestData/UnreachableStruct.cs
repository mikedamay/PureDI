using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class UnreachableStruct
    {
        public ActualStruct actualStruct;
    }
    [Bean]
    public struct ActualStruct
    {
        [BeanReference] public SomeChildClass someChild;
    }
    [Bean]
    public class SomeChildClass
    {
    }
}