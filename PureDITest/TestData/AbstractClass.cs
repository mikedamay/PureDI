using PureDI;
using PureDI.Attributes;

namespace IOCCTest.TestData
{
    [Bean]
    public abstract class AbstractClass
    {
        
    }

    [Bean]
    public class ConcreteClass
    {
        
    }

    public abstract class AbstractClassToIgnore
    {
        
    }

    [Bean]
    public abstract class AbstractClass2
    {
        
    }
}