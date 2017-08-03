using com.TheDisappointedProgrammer.IOCC;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    public class TreeWithFields
    {
        [IOCCInjectedDependency]
        public ChildOne childOne;
    }
    [IOCCDependency]
    public class ChildOne
    {
        
    }
    public class NamedDependencies
    {
    }
}