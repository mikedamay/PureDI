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
            PDependencyInjector pdi = new PDependencyInjector(profiles: new[] {"myprofile"});

        //pdi.SetAssemblies(assembly.GetName().Name);
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies("IOCCTest.DuplicateTestData.Duplicate"
              ,assemblySpec: new AssemblySpec(assemblies: assembly)
              ).injectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsTrue(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldCreateTreeForSpecificOs()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.DuplicateTestData.Os.cs");
            PDependencyInjector pdi = new PDependencyInjector();
            Diagnostics diagnostics = pdi.CreateAndInjectDependencies(
              "IOCCTest.DuplicateTestData.Duplicate"
              ,assemblySpec: new AssemblySpec(assemblies: assembly)
              )
              .injectionState.Diagnostics;
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsFalse(diagnostics.Groups["DuplicateBean"]
              .Occurrences.Any(diag => ((dynamic)diag).Interface.Contains( "MuchoInterface")));
        }
    }
}