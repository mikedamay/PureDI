using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace TestCSharpProvider
{
    class Program
    {
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
            parms.GenerateInMemory = true;

            CompilerResults result 
              = csc.CompileAssemblyFromSource(parms, "class myclass {}");
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
