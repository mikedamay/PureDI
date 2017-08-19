using System;
using System.IO;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest
{
    internal static class Utils
    {
        public static
            (dynamic result, IOCCDiagnostics diagnostics)
            CreateAndRunAssembly(string nameSpace, string className)
        {
            var iocc = CreateAssembly(nameSpace, className);
            object rootBean = iocc.GetOrCreateObjectTree(
                $"IOCCTest.{nameSpace}.{className}"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            dynamic result = (IResultGetter)rootBean;
            return (result, diagnostics);
        }

        public static SimpleIOCContainer CreateAssembly(string nameSpace, string className)
        {
            string codeText = GetResource(
                $"IOCCTest.{nameSpace}.{className}.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            SimpleIOCContainer iocc = new SimpleIOCContainer();
            iocc.SetAssemblies(assembly.GetName().Name);
            return iocc;
        }

        public static string GetResource(string resourceName)
        {
            try
            {
                using (Stream s
                    = typeof(Utils).Assembly.GetManifestResourceStream(resourceName))
                using (StreamReader sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (ArgumentNullException aue)
            {
                throw new Exception(
                    $"Most likely the file {resourceName} has not been created or has not been marked as an embedded resource in the VS project"
                    , aue);
            }
        }
    }
}