using System.Dynamic;
using PureDI;
using PureDI.Attributes;
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