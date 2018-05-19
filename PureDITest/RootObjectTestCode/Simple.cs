using System.Dynamic;
using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{  
    public class Simple : IOCCTest.TestCode.IResultGetter
    {
        [BeanReference] private Child child = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }
    }

    [Bean]
    public class Child
    {
        
    }
}