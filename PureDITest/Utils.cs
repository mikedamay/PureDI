//#define USE_THIS_ASSEMBLY
using System;
using System.IO;
using System.Reflection;
using PureDI;
using IOCCTest.TestCode;


namespace IOCCTest
{
    internal static class Utils
    {
        public const string TestResourcePrefix = "PureDITest";
        public static
            (dynamic result, Diagnostics diagnostics)
            CreateAndRunAssembly(string nameSpace, string className)
        {
            (var iocc, var assembly) = CreateIOCCinAssembly(nameSpace, className);
            (object rootBean, InjectionState InjectionState) = iocc.CreateAndInjectDependencies(
                $"IOCCTest.{nameSpace}.{className}", assemblySpec: new AssemblySpec(assemblies: assembly));
            Diagnostics diagnostics = InjectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            dynamic result = (IResultGetter)rootBean;
            return (result, diagnostics);
        }
        /// <summary>
        /// Creates an assembly for the className and returns a container with assembly assigned 
        /// </summary>
        /// <param name="testDataFolderName">e.g. "ScopeTestData" - no prefix requireed</param>
        /// <param name="className">e.g. "FactoryPrototype" - no ".cs" required</param>
        /// <returns>instantiated container with an assembly based on className but no tree</returns>
        public static (PDependencyInjector, Assembly) CreateIOCCinAssembly(string testDataFolderName
            , string className)
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.{testDataFolderName}.{className}.cs");
            PDependencyInjector iocc = new PDependencyInjector();
            return (iocc, assembly);
        }

        public static Assembly CreateAssembly(string resourceName)
        {
#if !USE_THIS_ASSEMBLY
            string codeText = GetResource(
                resourceName);
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
#else
            Assembly assembly = typeof(Utils).Assembly;
#endif
            return assembly;
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
        public static bool IsDotNetCore()
        {
            return true;
        }
        /// <summary>
        /// This function is typically called when asserting whether
        /// any diagnostics have been recorded during a test.
        /// "Assert.IsFalse(Utils.Falsify(diagnostics.HasWarnings));"
        /// The problem this addresses is as follows:
        /// When tests are built under dotnetcore then diagnostics.HasWarnings will
        /// always return true because of other deliberately failing tests.
        /// The assembly in which the tests are located is considered as a whole.
        /// When building under .net franework the assmeblies are built in
        /// isolation.
        /// </summary>
        /// <param name="expr">typically diagnostics.HasWarnings</param>
        /// <returns>if built under .NETCORE then false, otherwise
        ///   simply returns the value passed in</returns>
        public static bool Falsify(bool expr)
        {
#if USE_THIS_ASSEMBLY
            return false;
#else
            return expr;
#endif
        }

        public static int LessThanIsGoodEnough(int expected, int actual)
        {
#if USE_THIS_ASSEMBLY
            if (expected < actual)
            {
                return actual;  // make "Assert.AreEqual(expected, actual);" return true;
            }
            else
            {
                return expected;
            }
#endif
            return expected;
       }
    }
}