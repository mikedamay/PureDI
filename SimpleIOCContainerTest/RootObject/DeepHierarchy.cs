using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.RootObject
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