using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.DifficultTypeTestData
{
    [IOCCIgnore]
    public abstract class NonBeanFactoryBase : IFactory
    {
        public abstract object Execute(BeanFactoryArgs args);
    }
    [Bean]
    public class IgnoredNonBeanFactory : NonBeanFactoryBase
    {
        public override object Execute(BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}