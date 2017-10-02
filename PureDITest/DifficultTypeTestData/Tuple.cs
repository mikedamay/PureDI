using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class Tuple : IResultGetter
    {
        [BeanReference]
        public (AnotherType1 t1, AnotherType1 t2) stuff;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Stuff = stuff;
            return eo;
        }
    }

    [Bean]
    public class AnotherType1
    {
        
    }
}