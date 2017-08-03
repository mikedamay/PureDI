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
            string codeText = GetResource("IOCCTest.TestData.TreeWithFields.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            System.Diagnostics.Debug.WriteLine(
                $"There are {AppDomain.CurrentDomain.GetAssemblies().Length} assmblies loaded");
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(new List<Assembly>() {assembly});
            Assert.AreEqual(1, map.Keys.Count);

        }
        [TestMethod]
        public void ShouldCrewateTypeMapFromNamedDependencies()
        {
            string codeText = GetResource("IOCCTest.TestData.NamedDependencies.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            System.Diagnostics.Debug.WriteLine(
                $"There are {AppDomain.CurrentDomain.GetAssemblies().Length} assmblies loaded");
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(new List<Assembly>() {assembly});
            Assert.AreEqual(6, map.Keys.Count);
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("NamedDependencies", "dep-name-abc"), "NamedDependencies"}
                ,{("NamedDependencies1", "dep-name-xyz"), "NamedDependencies1"}
                ,{("ISecond", "dep-name-xyz"), "NamedDependencies1"}
                ,{("NamedDependencies2", "dep-name-def"), "NamedDependencies2"}
                ,{("INamedDependencies", "dep-name-def"), "NamedDependencies2"}
                ,{("ISecond", "dep-name-def"), "NamedDependencies2"}
            };
            CompareMaps(map, mapExpected);

        }

        private static void CompareMaps(IDictionary<ValueTuple<Type, string>, TypeHolder> map, IDictionary<ValueTuple<string, string>, string> mapExpected)
        {
            foreach ((var interfaceType, var dependencyName) in map.Keys)
            {
                Assert.IsTrue(mapExpected.ContainsKey((interfaceType.Name, dependencyName)));
                switch (map[(interfaceType, dependencyName)].Content)
                {
                    case System.Type t:
                        Assert.AreEqual(mapExpected[(interfaceType.Name, dependencyName)], t.Name);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
            }
        }
        [Ignore]
        [TestMethod]
        public void ShouldCreateTypeMapForTypesInADeepHierarchy()
        {
            string codeText = GetResource(
              "IOCCTest.TestData.DependencyHierarchy.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(new List<Assembly>() { assembly });
            Assert.AreEqual(8, map.Keys.Count);
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
            CompareMaps(map, mapExpected);
        }

        [TestMethod]
        public void OutputTypeMap_NotReallyATest()
        {
            BuildAndOutputTypeMap("IOCCTest.TestData.DependencyHierarchy.cs");
        }

        private void BuildAndOutputTypeMap(string resourceName)
        {
            Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(resourceName));
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(new List<Assembly>() { assembly });
            string str = MapToString(map);
            System.Diagnostics.Debug.WriteLine(str);
        }

        private string MapToString(IDictionary<(Type type, string name), TypeHolder> map)
        {
            StringBuilder sb = new StringBuilder();
            void AddMapEntry((Type type, string name) key)
            {
                (Type dependencyInterface, string dependencyName) = key;
                var dependencyImplementation = map[key].Content as Type;
                sb.Append($@"{{(""{dependencyInterface}"", ""{dependencyName}""),""{dependencyImplementation}""}}"
                  + Environment.NewLine);
                
            }
            sb.Append("IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()"
              + Environment.NewLine);
            sb.Append("{" + Environment.NewLine);

            var iter = map.Keys.GetEnumerator();
            iter.MoveNext();
            sb.Append("\t");
            AddMapEntry(iter.Current);
            while (iter.MoveNext())
            {
                sb.Append("\t,");
                AddMapEntry(iter.Current);
            }
            sb.Append("};" + Environment.NewLine);
            iter.Dispose();
            return sb.ToString();
        }

        /// <summary>
        /// Typically gets code to comprise the assembly being created dynamically
        /// </summary>
        /// <remarks>
        /// use ildasm to find the resource names in the manifest 
        /// </remarks>
        /// <param name="resourceName">IOCCTest.TestData.xxx.cs</param>
        /// <returns>complete code capable of building an assembly</returns>
        private string GetResource(string resourceName)
        {
            using (Stream s
                = this.GetType().Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
