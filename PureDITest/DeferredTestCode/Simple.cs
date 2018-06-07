using PureDI.Attributes;

namespace IOCCTest.DeferredTestCode
{
    [Bean]
    public class Simple
    {
        [BeanReference]public Simple SimpleChild;
        [BeanReference] public NotSimple NotSimpleChild;
    }

    [Bean]
    public class NotSimple
    {
        
    }
}