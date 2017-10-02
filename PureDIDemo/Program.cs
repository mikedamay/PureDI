using System;
using System.Threading;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            PDependencyInjector pdi = new PDependencyInjector();
            TodoProcessor tdp = pdi.CreateAndInjectDependencies<TodoProcessor>().rootBean;
            tdp.Process();
            Thread.Sleep(5000);
            Console.Read();
        }
    }
}
