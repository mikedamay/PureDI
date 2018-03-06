using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class Dynamic : IResultGetter
    {
        [BeanReference] dynamic anotherDynamic;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }

    [Bean]
    public class AnotherDynamic
    {
        
    }
}