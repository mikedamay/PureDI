using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using IOCCTest;

namespace TestCSharpProvider
{
    class Program
    {
        static void MainX(string[] args)
        {
            TypeMapBuilderTest tbt = new TypeMapBuilderTest();
            tbt.TestAssembly();

        }
        static void Main(string[] args)
        {
            Assembly assembly;
            var csc = new CSharpCodeProvider();
            var parms = new CompilerParameters(
              new string[]
              {"mscorlib.dll", "System.Core.dll", "System.dll"
              }
              );
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = false;

            //CompilerResults result
            //  = csc.CompileAssemblyFromSource(parms, "class myclass {}");
            CompilerResults result
              = csc.CompileAssemblyFromFile(parms, "c:\\projects\\mike.cs");
            if (result.Errors.Count > 0)
            {
                throw new Exception("compilation failed:" + Environment.NewLine
                  + result.Errors[0]);
            }
            else
            {
                assembly = result.CompiledAssembly;
            }
        }
    }
}
