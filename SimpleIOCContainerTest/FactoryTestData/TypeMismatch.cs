using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class TypeMismatchFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return 42;
        }
    }
    [IOCCBean]
    public class TypeMismatch : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(TypeMismatchFactory))] public string memberString;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            return eo;
        }
    }
}