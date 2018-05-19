using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
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