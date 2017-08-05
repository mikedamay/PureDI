using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCDependency]
    public abstract class AbstractClass
    {
        
    }

    [IOCCDependency]
    public class ConcreteClass
    {
        
    }

    public abstract class AbstractClassToIgnore
    {
        
    }

    [IOCCDependency]
    public abstract class AbstractClass2
    {
        
    }
}