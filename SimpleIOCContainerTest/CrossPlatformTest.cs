using System.Collections.Generic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class CrossPlatformTest
    {
        [TestMethod]
        public void ShouldCreateLinuxTypesOnLinux()
        {
            SimpleIOCContainer sic = Utils.CreateIOCCinAssembly("TestData", "CrossPlatform");
            (object rootBean, InjectionState injectionState)
                = sic.CreateAndInjectDependenciesWithString("IOCCTest.TestData.CrossPlatform");
            IResultGetter result = rootBean as IResultGetter;
#if WINDOWSTEST
            Assert.IsNotNull(result.GetResults().Windows);
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