using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            using (Stream stream = typeof(IOCC).Assembly.GetManifestResourceStream(
                "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;
                
            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, IOCC.DEFAULT_PROFILE, IOCC.OS.Any);
            Assert.AreEqual(2, diagnostics.Groups["InvalidBean"].Errors.Count);

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
            CommonTypeMapTest("IOCCTest.TestData.CheckProfileAndOs.cs", mapExpected, "someProfile", IOCC.OS.Windows);
        }
        [TestMethod]
        public void ShouldIgnoreNamedProfileAndOsWhenNoParamsPassed()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.CheckProfileAndOs9", ""),"IOCCTest.TestData.CheckProfileAndOs9"}
            };
            CommonTypeMapTest("IOCCTest.TestData.CheckProfileAndOs.cs", mapExpected
              , IOCC.DEFAULT_PROFILE, IOCC.OS.Any);
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
            using (Stream stream = typeof(IOCC).Assembly.GetManifestResourceStream(
                "com.TheDisappointedProgrammer.IOCC.Docs.DiagnosticSchema.xml"))
            {
                diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            }
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, IOCC.DEFAULT_PROFILE, IOCC.OS.Any);
            Assert.AreEqual(4, map.Keys.Count);
            Assert.AreEqual(1, diagnostics.Groups["DuplicateBean"].Errors.Count);
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
            void BuildAndOutputTypeMap(string resourceName, string profile, IOCC.OS os)
            {
                Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(resourceName));
                IOCCDiagnostics diagnostics = new IOCCDiagnostics();
                var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                  new List<Assembly>() { assembly }, ref diagnostics, profile, os);
                string str = map.OutputToString();
                System.Diagnostics.Debug.WriteLine(str);
            }
            // change the resource name arg in the call below to generate the code
            // for the specific test
            BuildAndOutputTypeMap("IOCCTest.TestData.CheckProfileAndOs.cs", IOCC.DEFAULT_PROFILE, IOCC.OS.Any);
        }

        private void CommonTypeMapTest(string testDataName
          , IDictionary<(string, string), string> mapExpected
          , string profile = IOCC.DEFAULT_PROFILE, IOCC.OS os = IOCC.OS.Any)
        {
            string codeText = GetResource(testDataName);
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCCDiagnostics diagnostics = new IOCCDiagnostics();
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                new List<Assembly>() { assembly }, ref diagnostics, profile, os);
            Assert.AreEqual(mapExpected.Keys.Count, map.Keys.Count);
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
        private static string GetResource(string resourceName)
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
            foreach ((var interfaceType, var dependencyName) in map.Keys)
            {
                Assert.IsTrue(mapExpected.ContainsKey((interfaceType.FullName, dependencyName)));
                Assert.AreEqual(mapExpected[(interfaceType.FullName, dependencyName)]
                    , map[(interfaceType, dependencyName)].FullName);
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
                (Type dependencyInterface, string dependencyName) = key;
                var dependencyImplementation = map[key] as Type;
                sb.Append($@"{{(""{dependencyInterface}"", ""{dependencyName}""),""{dependencyImplementation}""}}"
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
}
