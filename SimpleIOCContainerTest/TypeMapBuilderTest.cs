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
                = this.GetType().Assembly.GetManifestResourceStream("IOCCTest.TestData.TreeWithFields.cs"))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
