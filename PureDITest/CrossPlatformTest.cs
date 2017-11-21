using System.Collections.Generic;
using System.Reflection;
using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace IOCCTest
{
    [TestClass]
    public class CrossPlatformTest
    {
        /// <summary>
        /// usage:
        /// windows:
        ///     dotnet run
        /// linux
        ///     dotnet run -c LinuxTest
        /// Mac
        ///     dotnet run -c MacosTest
        /// </summary>
        [TestMethod]
        public void ShouldCreateLinuxTypesOnLinux()
        {
            (PDependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("TestData", "CrossPlatform");
            (object rootBean, InjectionState injectionState)
              = pdi.CreateAndInjectDependencies("IOCCTest.TestData.CrossPlatform"
              , assemblySpec: new AssemblySpec(assemblies: assembly));
            IResultGetter result = rootBean as IResultGetter;
#if WINDOWSTEST
            Assert.IsNotNull(result.GetResults().Windows, "try \"dotnet run -c LinuxTest\" or \"dotnet run -c MacOsTest\"");
#endif
#if LINUXTEST
            Assert.IsNotNull(result.GetResults().Linux);
#endif
#if MACOSTEST
            Assert.IsNotNull(result.GetResults().Macos);
#endif
        }
    }
}
