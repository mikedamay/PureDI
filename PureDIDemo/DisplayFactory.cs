using System;
using PureDI;
namespace SimpleIOCCDemo
{
    [Bean]
    internal class DisplayFactory : IFactory
    {
        [BeanReference] private PDependencyInjector iocContainer = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return iocContainer.CreateAndInjectDependencies<VSOutputWindow>(injectionState, rootBeanName: "outputWindow");
            }
            else
            {
                return iocContainer.CreateAndInjectDependencies<ConsoleDisplay>(injectionState);
            }
        }
    }
}