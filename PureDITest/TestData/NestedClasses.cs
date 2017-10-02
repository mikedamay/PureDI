using PureDI;

namespace IOCCTest.TestData
{
    public class NestedClasses
    {
        [Bean]
        private class NestedDependency
        {
            
        }

        [Bean]
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
