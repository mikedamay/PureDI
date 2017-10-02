using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    public class DontKnowWhy : IgnoreBaseAttribute
    {
        
    }
    [DontKnowWhy]
    public interface IInterface
    {
        
    }
    [Bean]
    public class Ignore : IResultGetter
    {
        public dynamic GetResults() => null;
    }

    [Bean]
    public class Bean1 : IInterface
    {

    }
    [Bean]
    public class Bean2 : IInterface
    {

    }
}