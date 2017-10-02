using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.CaseSensitivityTestData
{
    [Bean]
    public class Simple : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = "Simple";
            return eo;
        }
    }
    [Bean]
    public class simple : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = "simple";
            return eo;
        }
    }
}