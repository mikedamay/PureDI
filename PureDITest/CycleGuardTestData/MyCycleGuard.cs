using System.Dynamic;
using IOCCTest.TestCode;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.CycleGuardTestData
{
    [Bean]
    public class MyCycleGuard : IResultGetter
    {
        [BeanReference(Factory = typeof(CycleFactory))]
        private MyDependency dependency = null;

        private MyCycleGuard()
        {
            _ = dependency;
        }

        public dynamic GetResults()
        {
            dynamic results = new ExpandoObject();
            results.Dependency = dependency;
            return results;
        }
    }

    [Bean]
    internal class MyDependency
    {
        [BeanReference] private MyCycleGuard cycleGuard = null;

        private MyDependency()
        {
            _ = cycleGuard;
        }
    }
    [Bean]
    internal class CycleFactory : IFactory
    {
        [BeanReference] private PDependencyInjector injector = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<MyDependency>(injectionState);
        }
    }
}