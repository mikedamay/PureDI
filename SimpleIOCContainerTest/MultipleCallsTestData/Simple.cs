using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class Simple : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
    [Bean]
    public class Further : IResultGetter
    {
        [BeanReference] private Simple simple = null;

        Further()
        {
            var x = simple;
        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Simple = simple;
            return eo;
        }
        
    }
}