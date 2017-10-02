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

    [Bean(OS = PDependencyInjector.OS.MacOS)]
    public class OsImplMac : MuchoInterface
    {

    }
    [Bean(OS = PDependencyInjector.OS.Windows)]
    public class OsImplWindows : MuchoInterface
    {

    }
    [Bean(OS = PDependencyInjector.OS.Linux)]
    public class OsImplLinux : MuchoInterface
    {

    }
}