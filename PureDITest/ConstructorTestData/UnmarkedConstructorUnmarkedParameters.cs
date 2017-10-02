using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class UnmarkedConstructorUnmarkedParameters : IResultGetter
    {
        public UnmarkedConstructorUnmarkedParameters()
        {
            
        }

        public UnmarkedConstructorUnmarkedParameters(int i)
        {
            
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}