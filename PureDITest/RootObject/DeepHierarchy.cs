using PureDI;
using PureDI.Attributes;

namespace IOCCTest.rootBean
{
    [Bean]
    public class DeepHierarchy
    {
        
    }

    [Bean]
    public class SomeUser
    {
        [BeanReference] public DeepHierarchy deep;
    }
}