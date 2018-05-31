using System;
using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDemo
{
    [Bean]
    internal class DisplayFactory : IFactory
    {
        [BeanReference] private DependencyInjector iocContainer = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return iocContainer.CreateAndInjectDependencies<VSOutputWindow>(injectionState
                  , rootBeanSpec: new RootBeanSpec(rootBeanName: "outputWindow"));
            }
            else
            {
                return iocContainer.CreateAndInjectDependencies<ConsoleDisplay>(injectionState);
            }
        }
    }
}