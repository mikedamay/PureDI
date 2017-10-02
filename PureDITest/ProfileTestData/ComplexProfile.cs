using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ProfileTestData
{
    [Bean(Profile="P1")]
    public class ComplexProfile : IResultGetter
    {
        [BeanReference] private ChildP2 childP2;
        [BeanReference] private ChildP3 childP3;
        [BeanReference] private ChildP4 childP4;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildP2 = childP2;
            eo.ChildP3 = childP3;
            eo.ChildP4 = childP4;
            return eo;
        }
    }
    [Bean(Profile="P4")]
    internal class ChildP4
    {
    }

    [Bean(Profile="P3")]
    internal class ChildP3
    {
    }

    [Bean(Profile="P2")]
    internal class ChildP2
    {
    }
}