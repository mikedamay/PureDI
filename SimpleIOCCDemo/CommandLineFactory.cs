using System;
using System.Linq;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [Bean]
    public class CommandLineFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
        }
    }
}