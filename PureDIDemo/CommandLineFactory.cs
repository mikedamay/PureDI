using System;
using System.Linq;
using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDemo
{
    [Bean]
    public class CommandLineFactory : IFactory
    {
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (Environment.GetCommandLineArgs().Skip(1).FirstOrDefault(), injectionState);
        }
    }
}