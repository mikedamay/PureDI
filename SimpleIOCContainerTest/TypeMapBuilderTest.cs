using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest
{
    [TestClass]
    public class TypeMapBuilderTest
    {
        [TestMethod]
        public void ShouldCrewateTypeMapFromThisAssembly()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.ChildOne", ""),"IOCCTest.TestData.ChildOne"}
            };
            CommonTypeMapTest("IOCCTest.TestData.TreeWithFields.cs", mapExpected);
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
            CommonTypeMapTest("IOCCTest.TestData.NamedDependencies.cs", mapExpected);
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
            CommonTypeMapTest("IOCCTest.TestData.DependencyHierarchy.cs", mapExpected);
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
            CommonTypeMapTest("IOCCTest.TestData.NestedClasses.cs", mapExpected);
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
            CommonTypeMapTest("IOCCTest.TestData.DuplicateInterfaces.cs", mapExpected);
        }

        [TestMethod]
        public void ShouldWarnOfAbstractClassAsBean()
        {
            Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(
                "IOCCTest.TestData.AbstractClass.cs"));
            IOCCDiagnostics diagnostics;
            using (Stream stream = typeof(SimpleIOCContainer).Assembly.GetManifestResourceStream(
                "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;
                
            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS.Any);
            Assert.AreEqual(2, diagnostics.Groups["InvalidBean"].Occurrences.Count);

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
            CommonTypeMapTest("IOCCTest.TestData.CheckProfileAndOs.cs", mapExpected, "someProfile", SimpleIOCContainer.OS.Windows);
        }
        [TestMethod]
        public void ShouldIgnoreNamedProfileAndOsWhenNoParamsPassed()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.CheckProfileAndOs9", ""),"IOCCTest.TestData.CheckProfileAndOs9"}
            };
            CommonTypeMapTest("IOCCTest.TestData.CheckProfileAndOs.cs", mapExpected
              , SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS.Any);
        }
        [TestMethod]
        public void ShouldCreateEmptyTypeMapForAsseemblyWithNoDependencies()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
            };
            CommonTypeMapTest("IOCCTest.TestData.BlankAssembly.cs", mapExpected);
            
        }

        [TestMethod]
        public void ShouldWarnOfDuplicateBeans()
        {
            Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(
                "IOCCTest.TestData.DuplicateBeans.cs"));
            IOCCDiagnostics diagnostics;
            using (Stream stream = typeof(SimpleIOCContainer).Assembly.GetManifestResourceStream(
                "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS.Any);
            Assert.AreEqual(4, map.Keys.Count);
            Assert.AreEqual(1, diagnostics.Groups["DuplicateBean"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldCreateTypeMapForStruct()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.StructDependency", ""),"IOCCTest.TestData.StructDependency"}
            };
            CommonTypeMapTest("IOCCTest.TestData.StructDependency.cs", mapExpected);

        }

        [TestMethod]
        public void ShouldRecognizeConnectionsAcrossAssemblies()
        {
            Assembly assemblyInterface = new AssemblyMaker().MakeAssembly(GetResource(
                "IOCCTest.TestData.InterfaceClass.cs"), TargetAssemblyName : "Mike", InMemory : false);
            Assembly assemblyImplementation = new AssemblyMaker().MakeAssembly(GetResource(
                "IOCCTest.TestData.ImplementationClass.cs"), ExtraAssemblies: new [] { "Mike"}, InMemory: false);
            IOCCDiagnostics diagnostics;
            using (Stream stream = typeof(SimpleIOCContainer).Assembly.GetManifestResourceStream(
                "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assemblyInterface, assemblyImplementation }
              , ref diagnostics, SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS.Any);
            Assert.AreEqual(3, map.Keys.Count);
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericDeclarations()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.Generic`1", ""),"IOCCTest.TestData.Generic`1"}
                ,{("IOCCTest.TestData.GenericUser", ""),"IOCCTest.TestData.GenericUser"}
                ,{("IOCCTest.TestData.GenericChild", ""),"IOCCTest.TestData.GenericChild"}
                ,{("IOCCTest.TestData.Generic`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", ""),"IOCCTest.TestData.GenericChild"}
            };
            CommonTypeMapTest("IOCCTest.TestData.Generic.cs", mapExpected);

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
            void BuildAndOutputTypeMap(string resourceName, string profile, SimpleIOCContainer.OS os)
            {
                Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(resourceName));
                IOCCDiagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
                var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                    new List<Assembly>() {assembly}, ref diagnostics, profile, os);
                string str = map.OutputToString();
                System.Diagnostics.Debug.WriteLine(str);
            }
            // change the resource name arg in the call below to generate the code
            // for the specific test
            BuildAndOutputTypeMap("IOCCTest.TestData.IgnoreHelper.cs", SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS.Any);
        }
        /// <summary>
        /// Not currently used.
        /// wanted to run tests in another app domain.  Hoped to have access to the assembly
        /// via reflection in this domain but that, not surprisingly, is not how it works.
        /// would have to run the test in the other domain and drag back the results
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public (AppDomain, Assembly) CreateAssemblyInNewAppDomain(string resourceName)
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
        public static void CommonTypeMapTest(string testDataName
          , IDictionary<(string, string), string> mapExpected
          , string profile = SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.OS os = SimpleIOCContainer.OS.Any)
        {
            string codeText = GetResource(testDataName);
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCCDiagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, profile, os);
            Assert.AreEqual(mapExpected.Keys.Count, map.Keys.Count);
            Assert.IsFalse(diagnostics.HasWarnings);
            Assert.IsFalse(diagnostics.HasErrors);
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
        /// <summary>
        /// makes sure that all elements in the map built by the TypeMapBuilder
        /// are found in mapExpected.  The reverse check is not done.
        /// </summary>
        /// <param name="map">generated by the TypeMapBuilder</param>
        /// <param name="mapExpected">expected results of test</param>
        private static void CompareMaps(IDictionary<(Type, string), Type> map
          , IDictionary<(string, string), string> mapExpected)
        {
            foreach ((var interfaceType, var beanName) in map.Keys)
            {
                Assert.IsTrue(mapExpected.ContainsKey((interfaceType.GetIOCCName(), beanName)));
                Assert.AreEqual(mapExpected[(interfaceType.GetIOCCName(), beanName)]
                    , map[(interfaceType, beanName)].GetIOCCName());
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
