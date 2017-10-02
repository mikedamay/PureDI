using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.DifficultTypeTestData
{
    [Ignore]
    public abstract class NonBeanFactoryBase : IFactory
    {
        public abstract (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args);
    }
    [Bean]
    public class IgnoredNonBeanFactory : NonBeanFactoryBase
    {
        public override (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}