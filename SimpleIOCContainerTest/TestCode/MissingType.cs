using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    public class MissingType
    {
        [IOCCInjectedDependency] public int ii;
    }
}