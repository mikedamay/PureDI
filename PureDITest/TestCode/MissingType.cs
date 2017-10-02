using PureDI;

namespace IOCCTest.TestCode
{
    [Bean]
    public class MissingType
    {
        [BeanReference] public int ii;
    }
}