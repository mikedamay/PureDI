using PureDI;

namespace IOCCTest.DifficultTypeTestData
{
    public class NonBeanFactory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}