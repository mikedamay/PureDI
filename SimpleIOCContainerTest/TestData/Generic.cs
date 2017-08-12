using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCBean]
    public class Generic<T>
    {
        
    }
    [IOCCBean]
    public class GenericUser
    {
    }

    [IOCCBean]
    public class GenericChild : Generic<int>
    {
        
    }
}

