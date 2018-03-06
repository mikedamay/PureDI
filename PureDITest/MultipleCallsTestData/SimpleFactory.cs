using System.Dynamic;
using PureDI;
using PureDI.Attributes;
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
        [BeanReference] private PDependencyInjector injector = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<SimpleChild>(injectionState);
        }
    }
    [Bean]
    public class SimpleChild
    {
    }
}