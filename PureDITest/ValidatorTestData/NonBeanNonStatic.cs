using PureDI;
using PureDI.Attributes;

namespace IOCCTest.DifficultTypeTestData
{
    public class NonBeanNonStatic
    {
        [BeanReference] private int someInt;
    }
}