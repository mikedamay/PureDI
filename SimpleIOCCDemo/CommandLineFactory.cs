using System;
using System.Linq;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [IOCCBean]
    public class CommandLineFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
        }
    }
}