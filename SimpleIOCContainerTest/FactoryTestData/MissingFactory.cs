using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    public class FactoryWithoutFactoryAttribute {
    
    }
    [Bean]
    public class MissingFactory : IResultGetter
    {
        [BeanReference(Factory 
          = typeof(FactoryWithoutFactoryAttribute))] private int Abc;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}