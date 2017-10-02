using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class Fails : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
    [Bean]
    public class FurtherFails : IResultGetter
    {
        [BeanReference] private Fails simple = null;

        FurtherFails()
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