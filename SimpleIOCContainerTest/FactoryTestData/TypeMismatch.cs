using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    public class TypeMismatchFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return 42;
        }
    }
    [Bean]
    public class TypeMismatch : IResultGetter
    {
        [BeanReference(Factory = typeof(TypeMismatchFactory))] public string memberString;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}