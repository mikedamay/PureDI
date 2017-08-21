using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class ParameterNotInjectable : IResultGetter
    {
        [Constructor]
        public ParameterNotInjectable(
          [BeanReference] int abc)
        {
            
        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}