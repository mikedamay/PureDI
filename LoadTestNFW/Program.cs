                                                                  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new IOCCTest.LoadTest.LoadTest().TestLoad();
        }
    }
}
