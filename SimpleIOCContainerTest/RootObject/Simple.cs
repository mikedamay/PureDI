using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

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