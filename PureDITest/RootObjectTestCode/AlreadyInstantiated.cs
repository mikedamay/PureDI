using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    [Bean]
    public class AlreadyInstantiated
    {
        [BeanReference] public Childx Child;
    }

    [Bean]
    public class Childx
    {
        
    }
}