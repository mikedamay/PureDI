using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class SimpleFactory : IResultGetter
    {
        [BeanReference(Factory = typeof(ActualFactory))] private SimpleChild simpleChild;

        SimpleFactory()
        {
            simpleChild = null;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SimpleChild = simpleChild;
            return eo;
        }
    }
    [Bean]
    public class ActualFactory : IFactory
    {
        [BeanReference] private SimpleIOCContainer simpleIocContainer = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return simpleIocContainer.CreateAndInjectDependencies<SimpleChild>(injectionState);
        }
    }
    [Bean]
    public class SimpleChild
    {
    }
}