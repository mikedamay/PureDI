using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.CaseSensitivityTestData
{
    [Bean]
    public class Hierarchy : IResultGetter
    {
        [BeanReference] private Simple3 UpperCase = null;
        [BeanReference] private simple3 LowerCase = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.UpperCase = UpperCase.val;
            eo.LowerCase = LowerCase.val;
            return eo;
        }
        
    }
    [Bean]
    public class Simple3
    {
        public string val = "uppercase";
    }
    [Bean]
    public class simple3
    {
        public string val = "lowercase";
    }
}