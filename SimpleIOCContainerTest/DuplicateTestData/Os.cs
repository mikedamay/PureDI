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

    [Bean(OS = SimpleIOCContainer.OS.MacOS)]
    public class OsImplMac : MuchoInterface
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Windows)]
    public class OsImplWindows : MuchoInterface
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Linux)]
    public class OsImplLinux : MuchoInterface
    {

    }
}