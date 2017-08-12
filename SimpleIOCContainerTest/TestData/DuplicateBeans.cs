using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCBean]
    public class DuplicateBeans : Interface1
    {
        
    }
    [IOCCBean]
    public class DuplicateBeans2 : Interface1
    {
        
    }

    [IOCCBean]
    public class TrailingBean
    {
        
    }

    interface Interface1
    {
        
    }
}