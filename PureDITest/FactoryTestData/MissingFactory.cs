using System.Dynamic;
using PureDI;
using PureDI.Attributes;
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