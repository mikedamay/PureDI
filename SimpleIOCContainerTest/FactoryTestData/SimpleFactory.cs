using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class SimpleFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return args.FactoryParmeter;
        }
    }
    [IOCCBean]
    public class MyBean : IResultGetter
    {
        [IOCCBeanReference(Factory=typeof(SimpleFactory), FactoryParameter=10)]
        public int Abc;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Abc = Abc;
            return eo;
        }        
    }
}