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
        // currently assemblies are created but not unloaded. Too much pain-gain to fix this.
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
        /// <param name="CodeText">The complete text of an assembly</param>
        /// <param name="getAssemblyName"></param>
        /// <param name="targetAssemblyName"></param>
        /// <param name="ExtraAssemblies"></param>
        /// <returns>An assembly suitable for use in IOCC testing</returns>
        public Assembly MakeAssembly(string CodeText
            , string TargetAssemblyName = null, string[] ExtraAssemblies = null, bool InMemory = true)
        {
            Assembly assembly;
            var csc = new CSharpCodeProvider();
            var parms = new CompilerParameters(
              CombineReferencedAssemblies(new string[]
              {"mscorlib.dll", "System.Core.dll", "System.dll", "Microsoft.CSharp.dll"
              ,"SimpleIOCContainer.dll", "SimpleIOCContainerTest.dll"}
              , ExtraAssemblies)
              , SelectAssemblyName(TargetAssemblyName));
            parms.GenerateExecutable = false;
            parms.GenerateInMemory = InMemory;
            CompilerResults result = csc.CompileAssemblyFromSource(parms, CodeText);
            if (result.Errors.Count > 0)
            {
                throw new Exception("compilation failed:" + Environment.NewLine
                  + result.Errors[0]);
            }
            else
            {
                assembly = result.CompiledAssembly;
            }
            return assembly;
        }

        private static string SelectAssemblyName(string targetAssemblyName)
          => targetAssemblyName ?? RandomString(8);

        private static string[] CombineReferencedAssemblies(string[] standardAssemblies, string[] extraAssemblies)
          => standardAssemblies.Union(extraAssemblies ?? new string[0]).ToArray();
        
        // truly elegant
        // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
