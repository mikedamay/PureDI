using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(new List<Assembly>() {assembly});
            Assert.AreEqual(1, map.Keys.Count);

        }
        // TODO sort out mapp verification fully below
        [TestMethod]
        public void ShouldCrewateTypeMapFromNamedDependencies()
        {
            string codeText = GetResource("IOCCTest.TestData.NamedDependencies.cs");
            Assembly assembly = new AssemblyMaker().MakeAssembly(codeText);
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
            foreach ((var interfaceType, var dependencyName) in map.Keys)
            {
                Assert.IsTrue(mapExpected.ContainsKey((interfaceType.Name, dependencyName))
                  && map[(interfaceType, dependencyName)] != null);
            }

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
