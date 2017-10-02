using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using System.Dynamic;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class AccessByString : IResultGetter
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }

    public class NoAccess
    {
        
    }
}