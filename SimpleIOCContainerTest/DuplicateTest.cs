using System.Linq;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class DuplicateTest
    {
        [TestMethod]
        public void ShouldCreateTreeForVanillaBean()
        {
            (var result, var diagnostics) = CreateAndRunAssembly("DuplicateTestData", "Duplicate");
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldDetectDuplicatesForProfile()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.DuplicateTestData.Duplicate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(Profiles: new[] {"myprofile"}, Assemblies: new[] { assembly });

        //sic.SetAssemblies(assembly.GetName().Name);
            sic.CreateAndInjectDependenciesWithString("IOCCTest.DuplicateTestData.Duplicate", out var diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeForSpecificOs()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.DuplicateTestData.Os.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(Assemblies: new[] { assembly });
            //sic.SetAssemblies(assembly.GetName().Name);
            sic.CreateAndInjectDependenciesWithString("IOCCTest.DuplicateTestData.Duplicate", out var diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsFalse(diagnostics.Groups["DuplicateBean"]
              .Occurrences.Any(diag => ((dynamic)diag).Interface.Contains( "MuchoInterface")));
        }
    }
}