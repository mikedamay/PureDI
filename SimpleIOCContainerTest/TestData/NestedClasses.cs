using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    public class NestedClasses
    {
        [IOCCBean]
        private class NestedDependency
        {
            
        }

        [IOCCBean]
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
