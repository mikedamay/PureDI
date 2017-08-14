using com.TheDisappointedProgrammer.IOCC;

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
    public class TypeMismatch
    {
        [IOCCBeanReference(Factory = typeof(TypeMismatchFactory))] public string memberString;
    }
}