using PureDI;

namespace IOCCTest.DifficultTypeTestData
{
    public class NonBeanNonStatic
    {
        [BeanReference] private int someInt;
    }
}