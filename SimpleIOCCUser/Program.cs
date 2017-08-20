using System;
using System.Threading;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleIOCContainer sic = new SimpleIOCContainer();
            sic.SetAssemblies(typeof(SimpleIOCContainer).Assembly.GetName().Name);
            TodoProcessor tdp = sic.GetOrCreateObjectTree<TodoProcessor>();
            tdp.Process();
            Thread.Sleep(5000);
        }
    }
}
