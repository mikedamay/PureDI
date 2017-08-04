using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    public class NestedClasses
    {
        [IOCCDependency]
        private class NestedDependency
        {
            
        }

        [IOCCDependency]
        private class NestedDependencyWithAncestors : NestedParent, NestedInterface
        {
            
        }
        private class NestedParent
        {
            
        }

        private class NestedNonDependency
        {
            
        }
    }

    internal interface NestedInterface
    {
        
    }
}
