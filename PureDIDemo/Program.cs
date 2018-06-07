using System;
using System.Threading;
using PureDI;

namespace SimpleIOCCDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyInjector pdi = new DependencyInjector();
            TodoProcessor tdp = pdi.CreateAndInjectDependencies<TodoProcessor>().rootBean;
            tdp.Process();
            Thread.Sleep(5000);
            Console.Read();
        }
    }
}
