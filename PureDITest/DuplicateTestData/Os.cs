using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;
using PureDI.Public;

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

    [Bean(OS = Os.MacOs)]
    public class OsImplMac : MuchoInterface
    {

    }
    [Bean(OS = Os.Windows)]
    public class OsImplWindows : MuchoInterface
    {

    }
    [Bean(OS = Os.Linux)]
    public class OsImplLinux : MuchoInterface
    {

    }
}