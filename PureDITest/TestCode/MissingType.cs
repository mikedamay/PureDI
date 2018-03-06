using PureDI;
using PureDI.Attributes;

namespace IOCCTest.TestCode
{
    [Bean]
    public class MissingType
    {
        [BeanReference] public int ii;
    }
}