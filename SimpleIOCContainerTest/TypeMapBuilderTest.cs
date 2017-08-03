﻿using System;
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
            IOCCDiagnostics diagnostics = new IOCCDiagnostics();
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
              new List<Assembly>() {assembly}, ref diagnostics);
            Assert.AreEqual(1, map.Keys.Count);

        }
        [TestMethod]
        public void ShouldCrewateTypeMapFromNamedDependencies()
        {
            string codeText = GetResource("IOCCTest.TestData.NamedDependencies.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            System.Diagnostics.Debug.WriteLine(
                $"There are {AppDomain.CurrentDomain.GetAssemblies().Length} assmblies loaded");
            IOCCDiagnostics diagnostics = new IOCCDiagnostics();
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
              new List<Assembly>() {assembly}, ref diagnostics);
            Assert.AreEqual(6, map.Keys.Count);
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.NamedDependencies", "dep-name-abc"), "IOCCTest.TestData.NamedDependencies"}
                ,{("IOCCTest.TestData.NamedDependencies1", "dep-name-xyz"), "IOCCTest.TestData.NamedDependencies1"}
                ,{("IOCCTest.TestData.ISecond", "dep-name-xyz"), "IOCCTest.TestData.NamedDependencies1"}
                ,{("IOCCTest.TestData.NamedDependencies2", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
                ,{("IOCCTest.TestData.INamedDependencies", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
                ,{("IOCCTest.TestData.ISecond", "dep-name-def"), "IOCCTest.TestData.NamedDependencies2"}
            };
            CompareMaps(map, mapExpected);

        }

        [TestMethod]
        public void ShouldCreateTypeMapForTypesInADeepHierarchy()
        {
            string codeText = GetResource(
              "IOCCTest.TestData.DependencyHierarchy.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
            IOCCDiagnostics diagnostics = new IOCCDiagnostics();
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
              new List<Assembly>() { assembly }, ref diagnostics);
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
            void BuildAndOutputTypeMap(string resourceName)
            {
                Assembly assembly = new AssemblyMaker().MakeAssembly(GetResource(resourceName));
                IOCCDiagnostics diagnostics = new IOCCDiagnostics();
                var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
                  new List<Assembly>() { assembly }, ref diagnostics);
                string str = map.OutputToString();
                System.Diagnostics.Debug.WriteLine(str);
            }
            BuildAndOutputTypeMap("IOCCTest.TestData.DependencyHierarchy.cs");
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
        /// produces a string suitable for pasting into a test as
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
                iter.MoveNext();
                sb.Append("\t");
                AddMapEntry(iter.Current);
                while (iter.MoveNext())
                {
                    sb.Append("\t,");
                    AddMapEntry(iter.Current);
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
