using System;
using com.TheDisappointedProgrammer.IOCC;
namespace SimpleIOCCDemo
{
    [IOCCBean]
    internal class DisplayFactory : IOCCFactory
    {
        [IOCCBeanReference] private SimpleIOCContainer iocContainer;
        public object Execute(BeanFactoryArgs args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                return iocContainer.GetOrCreateObjectTree<VSOutputWindow>(beanName : "outputWindow");
            }
            else
            {
                return iocContainer.GetOrCreateObjectTree<ConsoleDisplay>();
            }
        }
    }
}