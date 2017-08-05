using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCDependency]
    public class DuplicateBeans : Interface1
    {
        
    }
    [IOCCDependency]
    public class DuplicateBeans2 : Interface1
    {
        
    }

    [IOCCDependency]
    public class TrailingBean
    {
        
    }

    interface Interface1
    {
        
    }
}