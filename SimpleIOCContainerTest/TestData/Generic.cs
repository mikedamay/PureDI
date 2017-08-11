using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCDependency]
    public class Generic<T>
    {
        
    }
    [IOCCDependency]
    public class GenericUser
    {
    }

    [IOCCDependency]
    public class GenericChild : Generic<int>
    {
        
    }
}

