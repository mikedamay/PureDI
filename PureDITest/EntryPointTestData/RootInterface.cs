using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.EntryPointTestData
{
    public interface RootInterface : IResultGetter
    {
        
    }

    [Bean]
    public class RootImplementation : RootInterface
    {
        public dynamic GetResults()
        {
            return (dynamic) null;
        }
    }
}