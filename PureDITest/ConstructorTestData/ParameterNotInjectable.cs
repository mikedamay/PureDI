using System.Dynamic;
using PureDI;
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