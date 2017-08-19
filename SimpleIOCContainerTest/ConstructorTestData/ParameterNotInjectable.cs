using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class ParameterNotInjectable : IResultGetter
    {
        [IOCCConstructor]
        public ParameterNotInjectable(
          [IOCCBeanReference] int abc)
        {
            
        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}