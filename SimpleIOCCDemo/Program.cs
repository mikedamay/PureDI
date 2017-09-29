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
            TodoProcessor tdp = sic.CreateAndInjectDependenciesSimple<TodoProcessor>();
            tdp.Process();
            Thread.Sleep(5000);
            Console.Read();
        }
    }
}
