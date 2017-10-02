using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ProfileTestData
{
    [Bean(Profile="Simple")]
    public class SimpleProfile : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }

        [BeanReference(Name="Other")] private SimpleChild child;
    }
    [Bean]
    internal class SimpleChild
    {
    }
}