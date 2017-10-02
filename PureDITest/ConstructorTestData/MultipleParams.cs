using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class MultipleParams : IResultGetter
    {
        private ParamOne paramOne;
        private ParamTwo paramTwo;
        [Constructor]
        public MultipleParams(
          [BeanReference]ParamOne one
          ,[BeanReference]ParamTwo two)
        {
            this.paramOne = one;
            this.paramTwo = two;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ParamOne = paramOne;
            eo.ParamTwo = paramTwo;
            return eo;
        }
    }

    [Bean]
    public class ParamOne
    {
        
    }
    [Bean]
    public class ParamTwo
    {
        
    }
}