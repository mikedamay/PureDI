using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace IOCCTest
{
    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/826398/is-it-possible-to-dynamically-compile-and-execute-c-sharp-code-fragments"/>
    /// </summary>
    internal class AssemblyMaker
    {
        // TODO currently assemblies are created but not unloaded.
        // there is a field AssemblyBuilderAccess.RunAndCollect 
        // which will cause assemblies to be garbage collected but it
        // it occurs in connection with System.Reflection.Emit and not
        // compiler services
        /// <summary>
        /// creates and compiles a DLL assembly based on codeText with the latest compiler
        /// </summary>
        /// <remarks>
        /// The compiler parameters (parms, below) may need tweaking if new external
        /// assemblies are referenced in codeText
        /// </remarks>
        /// <param name="codeText">The complete text of an assembly</param>
        /// <returns>An assembly suitable for use in IOCC testing</returns>
        public Assembly MakeAssembly(string codeText)
        {
            Assembly assembly;
            var csc = new CSharpCodeProvider();
            var parms = new CompilerParameters( new string[] { "mscorlib.dll", "System.Core.dll", "SimpleIOCContainer.dll"});
            parms.GenerateExecutable = false;
            CompilerResults result = csc.CompileAssemblyFromSource(parms, codeText);
            if (result.Errors.Count > 0)
            {
                throw new Exception("compilation failed");
            }
            else
            {
                assembly = result.CompiledAssembly;
            }
            return assembly;
        }
    }
}
