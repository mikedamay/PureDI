using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class MultipleParams : IResultGetter
    {
        private ParamOne paramOne;
        private ParamTwo paramTwo;
        [IOCCConstructor]
        public MultipleParams(
          [IOCCBeanReference]ParamOne one
          ,[IOCCBeanReference]ParamTwo two)
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

    [IOCCBean]
    public class ParamOne
    {
        
    }
    [IOCCBean]
    public class ParamTwo
    {
        
    }
}