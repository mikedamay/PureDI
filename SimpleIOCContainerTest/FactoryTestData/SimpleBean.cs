using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class SimpleFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return args.FactoryParmeter;
        }
    }
    [Bean]
    public class SimpleBean : IResultGetter
    {
        [BeanReference(Factory=typeof(SimpleFactory), FactoryParameter=10)]
        public int Abc;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Abc = Abc;
            return eo;
        }        
    }
}