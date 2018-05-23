using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    public class BaseClasses : SomeBaseClass, SomeInterface, SomeUnwantedInterface
    {
        
    }

    public interface SomeInterface
    {
        
    }

    public class SomeBaseClass
    {
        
    }

    [Ignore]
    public interface SomeUnwantedInterface
    {
        
    }

    [Bean]
    public class SomeClassUser
    {
        [BeanReference] public BaseClasses BaseClasses;
        [BeanReference] public SomeInterface SomeInterface;
        [BeanReference] public SomeBaseClass SomeBaseClass;
    }
}