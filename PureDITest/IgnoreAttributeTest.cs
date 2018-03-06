using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PureDI;
using PureDI.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [PureDI.Attributes.Ignore]
    public interface MyIgnoredReference
    {
        
    }
    [TestClass]
    public class IgnoreAttributeTest
    {
        [TestMethod]
        public void souldIgnoreAncestorsMarkedAsIOCCIgnore()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.IgnoreHelper", ""), "IOCCTest.TestData.IgnoreHelper"}
            };
            TypeMapBuilderTest.CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.IgnoreHelper.cs", mapExpected);

        }

        [TestMethod]
        public void ShouldCreateTreeForBeanMarkedAsIOCCIgnore()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.IgnoredBean", ""), "IOCCTest.TestData.IgnoredBean"}
                ,{("IOCCTest.TestData.ActualIgnoredBean", ""), "IOCCTest.TestData.ActualIgnoredBean"}
            };
            TypeMapBuilderTest.CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.IgnoredBean.cs", mapExpected);
            //Assembly assembly = Utils.CreateAssembly(
            //    $"{Utils.TestResourcePrefix}.TestData.IgnoredBean.cs");
            //Diagnostics diagnostics;
            //using (Stream stream = typeof(PDependencyInjector).Assembly.GetManifestResourceStream(
            //    $"{Common.ResourcePrefix}.Docs.DiagnosticSchema.xml"))
            //{
            //    diagnostics = new DiagnosticBuilder(stream).Diagnostics;

            //}
            //var map = new TypeMapBuilder().BuildTypeMapFromAssemblies(
            //    new List<Assembly>() { assembly }, ref diagnostics, new HashSet<string>(), PDependencyInjector.OS.Any);
            //Assert.AreEqual(Utils.LessThanIsGoodEnough(2, map.Keys.Count()), map.Keys.Count());
            //Assert.AreEqual(Utils.LessThanIsGoodEnough(1, diagnostics.Groups["DuplicateBean"].Occurrences.Count)
            //    , diagnostics.Groups["DuplicateBean"].Occurrences.Count);
        }
    }
}