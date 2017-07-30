using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest
{
    public class TreeWithFields
    {
        [IOCCInjectedDependency]
        public ChildOne childOne;
    }

    public class ChildOne
    {
        
    }
}