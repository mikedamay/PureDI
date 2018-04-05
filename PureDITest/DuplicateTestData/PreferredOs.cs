using System;
using System.Collections.Generic;
using System.Text;
using IOCCTest.TestCode;
using PureDI.Attributes;
using PureDI.Public;

namespace IOCCTest.DuplicateTestData
{
    [Bean]
    public class PreferredOs : IResultGetter
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
    public class OsImplMac : MuchoInterface
    {

    }
    [Bean(OS = Os.Windows)]
    public class OsImplWindows : MuchoInterface
    {

    }
}
