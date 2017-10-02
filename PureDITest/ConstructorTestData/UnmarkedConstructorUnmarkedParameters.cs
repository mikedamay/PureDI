using System.Dynamic;
using PureDI;
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