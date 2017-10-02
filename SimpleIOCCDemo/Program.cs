using System;
using System.Threading;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            PDependencyInjector sic = new PDependencyInjector();
            TodoProcessor tdp = sic.CreateAndInjectDependencies<TodoProcessor>().rootBean;
            tdp.Process();
            Thread.Sleep(5000);
            Console.Read();
        }
    }
}
