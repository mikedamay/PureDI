using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SimpleIOCCDocumentor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Start();
            Console.ReadLine();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
