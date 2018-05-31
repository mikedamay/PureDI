using System.Linq;
using System.Reflection;
using PureDI;
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
            DependencyInjector pdi = new DependencyInjector(profiles: new[] {"myprofile"});

        //pdi.SetAssemblies(assembly.GetName().Name);
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies("IOCCTest.DuplicateTestData.Duplicate"
              ,assemblies: new Assembly[] { assembly}
              ).injectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeForSpecificOs()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.DuplicateTestData.Os.cs");
            DependencyInjector pdi = new DependencyInjector();
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies(
              "IOCCTest.DuplicateTestData.Duplicate"
              ,assemblies: new Assembly[] { assembly}
              )
              .injectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsFalse(diagnostics.Groups["DuplicateBean"]
              .Occurrences.Any(diag => ((dynamic)diag).Interface1.Contains( "MuchoInterface")));
        }

        [TestMethod]
        public void ShouldPreferSpecificOs()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.DuplicateTestData.PreferredOs.cs");
            DependencyInjector pdi = new DependencyInjector();
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies(
              "IOCCTest.DuplicateTestData.PreferredOs"
              ,assemblies: new Assembly[] { assembly}
              )
              .injectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsFalse(diagnostics.Groups["DuplicateBean"]
              .Occurrences.Any(diag => ((dynamic)diag).Interface1.Contains( "MuchoInterface")));
            
        }
    }
}