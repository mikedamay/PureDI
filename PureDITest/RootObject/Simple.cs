using System.Dynamic;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.rootBean
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