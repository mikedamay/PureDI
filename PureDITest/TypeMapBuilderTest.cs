//#define USE_THIS_ASSEMBLY
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI;
using System.CodeDom.Compiler;
using System.Linq;
using PureDI.Common;
using Microsoft.CSharp;
using PureDI.Public;

namespace IOCCTest
{
    [TestClass]
    public class TypeMapBuilderTest
    {
#if NETCOREAPP2_0
        //[Microsoft.VisualStudio.TestTools.UnitTesting.Ignore]
        // this test uses CodeDom to build the assembly.
        // a) this fails on netstandard2.0 throwing a Unsupported Platform exception
        // b) we no longer use CodeDom preferring Roslyn Microsoft.CodeAnalysis
#endif
        //[TestMethod]
        public void TestAssembly()
        {
            Assembly assembly;
            var csc = new CSharpCodeProvider();
            var parms = new CompilerParameters(
              new []
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
            Assert.IsNotNull(assembly);
        }
        [TestMethod]
        public void ShouldCrewateTypeMapFromThisAssembly()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.ChildOne", ""),"IOCCTest.TestData.ChildOne"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.TreeWithFields.cs", mapExpected);
        }
        [TestMethod]
        public void ShouldCrewateTypeMapFromNamedDependencies()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.NamedDependencies", "dep-name-abc"), "IOCCTest.TestData.NamedDependencies"}
                ,{("IOCCTest.TestData.NamedDependencies1", "dep-name-xyz"), "IOCCTest.TestData.NamedDependencies1"}
                ,{("IOCCTest.TestData.ISecond", "dep-name-xyz"), "IOCCTest.TestData.NamedDependencies1"}
                ,{("IOCCTest.TestData.NamedDependencies2", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
                ,{("IOCCTest.TestData.INamedDependencies", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
                ,{("IOCCTest.TestData.ISecond", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.NamedDependencies.cs", mapExpected);
        }

        [TestMethod]
        public void ShouldCreateTypeMapForTypesInADeepHierarchy()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.SecondGenClass2", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.IThirdGen", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.ISecondGenWithAncestors2", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.IThirdGenA", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.ISecondGen3", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.IThirdGenB", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
                ,{("IOCCTest.TestData.DependencyHierarchy.IThirdGenC", ""),"IOCCTest.TestData.DependencyHierarchy.ForstGemClassWithManyAncestors8"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.DependencyHierarchy.cs", mapExpected);
        }

        [TestMethod]
        public void ShouldCreateTypeMapForNestedClass()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>
            {
                {("IOCCTest.TestData.NestedClasses+NestedDependency", ""),"IOCCTest.TestData.NestedClasses+NestedDependency"}
                ,{("IOCCTest.TestData.NestedClasses+NestedDependencyWithAncestors", ""),"IOCCTest.TestData.NestedClasses+NestedDependencyWithAncestors"}
                ,{("IOCCTest.TestData.NestedClasses+NestedParent", ""),"IOCCTest.TestData.NestedClasses+NestedDependencyWithAncestors"}
                ,{("IOCCTest.TestData.NestedInterface", ""),"IOCCTest.TestData.NestedClasses+NestedDependencyWithAncestors"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.NestedClasses.cs", mapExpected);
        }
        [TestMethod]
        public void ShouldCreateTypeMapForDuplicateInterfaces()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.DuplicateInterfaces.DuplicateInterfaces3", ""),"IOCCTest.TestData.DuplicateInterfaces.DuplicateInterfaces3"}
                ,{("IOCCTest.TestData.DuplicateInterfaces.FirstGen2", ""),"IOCCTest.TestData.DuplicateInterfaces.DuplicateInterfaces3"}
                ,{("IOCCTest.TestData.DuplicateInterfaces.SecondGen1", ""),"IOCCTest.TestData.DuplicateInterfaces.DuplicateInterfaces3"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.DuplicateInterfaces.cs", mapExpected);
        }

        [TestMethod]
        public void ShouldWarnOfAbstractClassAsBean()
        {
            Assembly assembly = Utils.CreateAssembly(
                $"{Utils.TestResourcePrefix}.TestData.AbstractClass.cs");
            Diagnostics diagnostics;
            using (Stream stream = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
                $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;
                
            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, new HashSet<string>(), Os.Any);
            Assert.AreEqual(Utils.LessThanIsGoodEnough(2, diagnostics.Groups["InvalidBean"].Occurrences.Count)
                , diagnostics.Groups["InvalidBean"].Occurrences.Count);

        }

        [TestMethod]
        public void ShouldFilterTypesBasedOnProfileAndOS()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.CheckProfileAndOs2", ""),"IOCCTest.TestData.CheckProfileAndOs2"}
                ,{("IOCCTest.TestData.CheckProfileAndOs3", ""),"IOCCTest.TestData.CheckProfileAndOs3"}
                ,{("IOCCTest.TestData.CheckProfileAndOs8", ""),"IOCCTest.TestData.CheckProfileAndOs8"}
                ,{("IOCCTest.TestData.CheckProfileAndOs9", ""),"IOCCTest.TestData.CheckProfileAndOs9"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.CheckProfileAndOs.cs", mapExpected
              , new HashSet<string> { "someProfile" }, Os.Windows);
        }
        [TestMethod]
        public void ShouldIgnoreNamedProfileAndOsWhenNoParamsPassed()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.CheckProfileAndOs9", ""),"IOCCTest.TestData.CheckProfileAndOs9"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.CheckProfileAndOs.cs", mapExpected
              , new HashSet<string>(), Os.Any);
        }
        [TestMethod]
        public void ShouldCreateEmptyTypeMapForAsseemblyWithNoDependencies()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.BlankAssembly.cs", mapExpected);
            
        }

        [TestMethod]
        public void ShouldWarnOfDuplicateBeans()
        {
            Assembly assembly = Utils.CreateAssembly(
                $"{Utils.TestResourcePrefix}.TestData.DuplicateBeans.cs");
            Diagnostics diagnostics;
            using (Stream stream = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
                $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, new HashSet<string>(), Os.Any);
            Assert.AreEqual(Utils.LessThanIsGoodEnough(4, map.Keys.Count()), map.Keys.Count());
            Assert.AreEqual(Utils.LessThanIsGoodEnough(1, diagnostics.Groups["DuplicateBean"].Occurrences.Count)
                , diagnostics.Groups["DuplicateBean"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldWarnOfInterfaceImplmentedByBaseExtendedByDerived()
        {
            Assembly assembly = Utils.CreateAssembly(
                $"{Utils.TestResourcePrefix}.TestData.BeanBase.cs");
            Diagnostics diagnostics;
            using (Stream stream = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
                $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, new HashSet<string>(), Os.Any);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.AreEqual(Utils.LessThanIsGoodEnough(3, map.Keys.Count()), map.Keys.Count());
            Assert.AreEqual(Utils.LessThanIsGoodEnough(2, diagnostics.Groups["DuplicateBean"].Occurrences.Count)
                , diagnostics.Groups["DuplicateBean"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldCreateTreeForInterfaceImplementedByBaseExtendedByDerivedWithNames()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("PureDITest.TestData.NamedBeanBase", ""),"PureDITest.TestData.NamedBeanBase"}
                ,{("PureDITest.TestData.INamedBean", ""),"PureDITest.TestData.NamedBeanBase"}
                ,{("PureDITest.TestData.ANamedBean", "derived"),"PureDITest.TestData.ANamedBean"}
                ,{("PureDITest.TestData.NamedBeanBase", "derived"),"PureDITest.TestData.ANamedBean"}
                ,{("PureDITest.TestData.INamedBean", "derived"),"PureDITest.TestData.ANamedBean"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.NamedBeanBase.cs", mapExpected);

        }
        [TestMethod]
        public void ShouldCreateTypeMapForStruct()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.StructDependency", ""),"IOCCTest.TestData.StructDependency"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.StructDependency.cs", mapExpected);
        }

#if !USE_THIS_ASSEMBLY
        [TestMethod]
        public void ShouldRecognizeConnectionsAcrossAssemblies()
        {
            Assembly assemblyInterface = new AssemblyMaker().MakeAssembly(GetResource(
                    $"{Utils.TestResourcePrefix}.TestData.InterfaceClass.cs"), TargetAssemblyName: "Mike"
                , InMemory: false);
            Assembly assemblyImplementation = new AssemblyMaker().MakeAssembly(GetResource(
                    $"{Utils.TestResourcePrefix}.TestData.ImplementationClass.cs")
                , ExtraAssemblies: new [] {assemblyInterface}, InMemory: false);
            Diagnostics diagnostics;
            using (Stream stream = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
                $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assemblyInterface, assemblyImplementation }
              , ref diagnostics, new HashSet<string>(), Os.Any);
            Assert.AreEqual(3, map.Keys.Count());
        }
#endif
        [TestMethod]
        public void ShouldCreateTreeForGenericDeclarations()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.Generic`1", ""),"IOCCTest.TestData.Generic`1"}
                ,{("IOCCTest.TestData.GenericUser", ""),"IOCCTest.TestData.GenericUser"}
                ,{("IOCCTest.TestData.GenericChild", ""),"IOCCTest.TestData.GenericChild"}
#if NETCOREAPP2_0
                ,{("IOCCTest.TestData.Generic`1[[System.Int32, System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", ""),"IOCCTest.TestData.GenericChild"}
#else
                ,{("IOCCTest.TestData.Generic`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", ""),"IOCCTest.TestData.GenericChild"}
#endif
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.Generic.cs", mapExpected);

        }

        [TestMethod]
        public void ShouldIgnoreNonBeanDerivedFromBean()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("PureDITest.TestData.NonBeanDerivedFromBean+BeanClass", ""),"PureDITest.TestData.NonBeanDerivedFromBean+BeanClass"}
            };
            CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.NonBeanDerivedFromBean.cs", mapExpected);

        }
        /// <summary>
        /// usage:
        /// 1) change the resource name in the code below to refer to
        ///    the resource for the specific test for which you want results
        /// 2) run this "test" to generate a block of code representing
        ///    expected results that can be pasted into the test
        /// </summary>
        [TestMethod]
        public void OutputTypeMap_NotReallyATest()
        {
            void BuildAndOutputTypeMap(string resourceName, string profile, Os os)
            {
                Assembly assembly = Utils.CreateAssembly(resourceName);
                Diagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
                var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                    new List<Assembly>() {assembly}, ref diagnostics, new HashSet<string>(), os);
                string str = map.OutputToString();
                System.Diagnostics.Debug.WriteLine(str);
            }
            // change the resource name arg in the call below to generate the code
            // for the specific test
            BuildAndOutputTypeMap($"{Utils.TestResourcePrefix}.TestData.NonBeanDerivedFromBean.cs", Constants.DefaultProfileArg, Os.Any);
        }

        /// <summary>
        /// Not currently used.
        /// wanted to run tests in another app domain.  Hoped to have access to the assembly
        /// via reflection in this domain but that, not surprisingly, is not how it works.
        /// would have to run the test in the other domain and drag back the results
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mapExpected"></param>
        /// <param name="profile"></param>
        /// <param name="os"></param>
        /// <returns></returns>
#if false // not supported on .NET core 2.0 - besides which it does not solve anything
        private (AppDomain, Assembly) CreateAssemblyInNewAppDomain(string resourceName)
        {
            AppDomain domain = null;
            try
            {
                AppDomainSetup domainSetup = new AppDomainSetup();
                domainSetup.ApplicationBase = Environment.CurrentDirectory;
                Evidence evidence = AppDomain.CurrentDomain.Evidence;
                domain = AppDomain.CreateDomain("newDomain", evidence, domainSetup);
                // the magic here is that this assembly appears to be automatically loaded
                // into the newly created domain
                Type type = typeof(Proxy);
                var value = (Proxy) domain.CreateInstanceAndUnwrap(
                    type.Assembly.FullName, type.GetIOCCName());
                Assembly assemblyUnderTest = value.GetAssemblyCount(resourceName);
                return (domain, assemblyUnderTest);
            }
            catch (Exception)
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
                throw;
            }
        }
#endif
        public static void CommonTypeMapTest(string resourceName
          , IDictionary<(string, string), string> mapExpected
          , ISet<string> profile = null
          , Os os = Os.Any)
        {
            Assembly assembly = Utils.CreateAssembly(resourceName);
            Diagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, profile, os);
            Assert.AreEqual(
              Utils.LessThanIsGoodEnough(mapExpected.Keys.Count, map.Keys.Count()), map.Keys.Count());
            Assert.IsFalse(Utils.Falsify(diagnostics.HasWarnings));
            CompareMaps(map, mapExpected);
        }



        /// <summary>
        /// Typically gets code to comprise the assembly being created dynamically
        /// </summary>
        /// <remarks>
        /// use ildasm to find the resource names in the manifest 
        /// </remarks>
        /// <param name="resourceName">IOCCTest.TestData.xxx.cs</param>
        /// <returns>complete code capable of building an assembly</returns>
        public static string GetResource(string resourceName)
        {
            using (Stream s
                = typeof(TypeMapBuilderTest).Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
        // the following is strange because the mechanism was first written to
        // search each of the actual results but on switching to .net core
        // we had to change to searching on the expected results which does not quite work
        /// <summary>
        /// makes sure that all elements in the map built by the TypeMapBuilder
        /// are found in mapExpected.  The reverse check is not done.
        /// </summary>
        /// <param name="map">generated by the TypeMapBuilder</param>
        /// <param name="mapExpected">expected results of test</param>
        private static void CompareMaps(IReadOnlyDictionary<(Type, string), Type> map
          , IDictionary<(string, string), string> mapExpected)
        {
            foreach ((var interfaceType, var beanName) in mapExpected.Keys)
            {
                (Type matchedType, string matchedBeanName) match
                  = map.Keys.FirstOrDefault(k => k.Item1.FullName == interfaceType
                  && k.Item2 == beanName);
                Assert.IsFalse(match.Equals( default(ValueTuple<Type, string>)));
                Assert.AreEqual(map[match].GetIOCCName()
                    , mapExpected[(interfaceType, beanName)]);
            }
        }
    }

    internal static class MapExtensions
    {
        /// <summary>
        /// produces a block of c# code suitable for pasting into test code as
        /// a set of expected results
        /// </summary>
        /// <param name="map">as produced by the TypeMapBuilder</param>
        /// <returns>set of expected results</returns>
        public static string OutputToString(this IDictionary<(Type type, string name), Type> map)
        {
            StringBuilder sb = new StringBuilder();
            void AddMapEntry((Type type, string name) key)
            {
                (Type beanInterface, string beanName) = key;
                var beanImplementation = map[key] as Type;
                sb.Append($@"{{(""{beanInterface.GetIOCCName()}"", ""{beanName}""),""{beanImplementation.GetIOCCName()}""}}"
                          + Environment.NewLine);

            }
            void AddMapEntriesToString()
            {
                var iter = map.Keys.GetEnumerator();
                if (iter.MoveNext())
                {
                    sb.Append("\t");
                    AddMapEntry(iter.Current);
                    while (iter.MoveNext())
                    {
                        sb.Append("\t,");
                        AddMapEntry(iter.Current);
                    }
                }
                iter.Dispose();
            }

            sb.Append(
                "IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()"
                + Environment.NewLine);
            sb.Append("{" + Environment.NewLine);
            AddMapEntriesToString();
            sb.Append("};" + Environment.NewLine);
            return sb.ToString();
        }
        public static string OutputToString(this IReadOnlyDictionary<(Type type, string name), Type> map)
        {
            StringBuilder sb = new StringBuilder();
            void AddMapEntry((Type type, string name) key)
            {
                (Type beanInterface, string beanName) = key;
                var beanImplementation = map[key] as Type;
                sb.Append($@"{{(""{beanInterface.GetIOCCName()}"", ""{beanName}""),""{beanImplementation.GetIOCCName()}""}}"
                          + Environment.NewLine);

            }
            void AddMapEntriesToString()
            {
                var iter = map.Keys.GetEnumerator();
                if (iter.MoveNext())
                {
                    sb.Append("\t");
                    AddMapEntry(iter.Current);
                    while (iter.MoveNext())
                    {
                        sb.Append("\t,");
                        AddMapEntry(iter.Current);
                    }
                }
                iter.Dispose();
            }

            sb.Append(
                "IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()"
                + Environment.NewLine);
            sb.Append("{" + Environment.NewLine);
            AddMapEntriesToString();
            sb.Append("};" + Environment.NewLine);
            return sb.ToString();
        }

    }
    public class Proxy : MarshalByRefObject
    {
        public Assembly GetAssemblyCount(string resourceName)
        {
            try
            {
                Assembly assembly = new AssemblyMaker().MakeAssembly("class xx {}");
                  //TypeMapBuilderTest.GetResource(resourceName));
                return assembly;
            }
            catch (Exception)
            {
                return null;
                // throw new InvalidOperationException(ex);
            }
        }
    }


}
