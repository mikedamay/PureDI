using PureDI;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    public class TreeWithFields
    {
        [BeanReference]
        public ChildOne childOne;
    }
    [Bean]
    public class ChildOne
    {
        
    }
}