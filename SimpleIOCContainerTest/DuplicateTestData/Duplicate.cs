using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DuplicateTestData
{
    [Bean]
    public class Duplicate : IResultGetter
    {
        public dynamic GetResults()
        {
            return null;
        }
    }

    public interface MuchoInterface
    {
        
    }

    [Bean]
    public class Vanilla : MuchoInterface
    {
        
    }

    [Bean(Profile = "myprofile")]
    public class WithProfile : MuchoInterface
    {

    }
    [Bean(Profile = "myprofile")]
    public class BadProfile : MuchoInterface
    {

    }
}