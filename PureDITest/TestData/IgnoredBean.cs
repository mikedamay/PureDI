using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.TestData
{
    [Bean]
    public class IgnoredBean : IResultGetter
    {
        [BeanReference] private ActualIgnoredBean ignored = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
    [Ignore]
    [Bean]
    public class ActualIgnoredBean
    {
        public string Val = "ActualIgnored";
    }
}