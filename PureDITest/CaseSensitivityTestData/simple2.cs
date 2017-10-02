using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.CaseSensitivityTestData
{
    [Bean]
    public class Simple2 : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = "Simple2";
            return eo;
        }
    }
    [Bean]
    public class simple2 : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = "simple2";
            return eo;
        }
    }
}