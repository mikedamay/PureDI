using com.TheDisappointedProgrammer.IOCC;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    public class TreeWithFields
    {
        [IOCCBeanReference]
        public ChildOne childOne;
    }
    [IOCCBean]
    public class ChildOne
    {
        
    }
    public class NamedDependencies
    {
    }
}