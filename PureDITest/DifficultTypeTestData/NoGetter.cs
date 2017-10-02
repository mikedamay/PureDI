using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class NoGetter : IResultGetter
    {
        private ActualNoGetter prop;
        [BeanReference] private ActualNoGetter Prop {set { prop = value; }}
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Prop = prop;
            return eo;
        }
    }
    [Bean]
    internal class ActualNoGetter
    {
    }
}