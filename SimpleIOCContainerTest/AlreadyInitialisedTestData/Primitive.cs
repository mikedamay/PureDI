using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.AlreadyInitialisedTestData
{
    [Bean]
    public class PrimitiveFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return 43;
        }
    }
    [Bean]
    public class Primitive : IResultGetter
    {
        [BeanReference(Factory =typeof(PrimitiveFactory))] private int val = 34;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = val;
            return eo;
        }
    }
}