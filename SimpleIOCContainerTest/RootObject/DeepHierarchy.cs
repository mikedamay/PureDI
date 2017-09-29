using com.TheDisappointedProgrammer.IOCC;

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