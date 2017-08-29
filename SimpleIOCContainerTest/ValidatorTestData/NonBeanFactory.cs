using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.DifficultTypeTestData
{
    public class NonBeanFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}