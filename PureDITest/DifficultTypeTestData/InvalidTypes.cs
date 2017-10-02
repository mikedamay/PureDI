using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class InvalidTypes : IResultGetter
    {
        [BeanReference] protected SomeMember[] someArray;
        [BeanReference] protected object someObject;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }

    [Bean]
    public class SomeMember
    {
        
    }
}