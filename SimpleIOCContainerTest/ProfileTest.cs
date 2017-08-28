using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class ProfileTest
    {
        [TestMethod]
        public void ShouldCreateTreeForSimpleProfile()
        {
            IOCCDiagnostics diagnostics = null;
            try
            {
                var sic = Utils.CreateIOCCinAssembly(
                    "ProfileTestData", "SimpleProfile");
                var result =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.SimpleProfile", out diagnostics, new[] { "Simple" }) as
                        IResultGetter;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNull(result?.GetResults().Child);
                Assert.IsTrue(diagnostics.HasWarnings);
            }
            catch (IOCCException iex)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldCreateTreeForComplexProfile()
        {
            IOCCDiagnostics diagnostics = null;
            try
            {
                var sic = Utils.CreateIOCCinAssembly(
                    "ProfileTestData", "ComplexProfile");
                var result =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics, new[] { "P1", "P2", "P3" }) as
                        IResultGetter;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNotNull(result?.GetResults().ChildP2);
                Assert.IsNotNull(result?.GetResults().ChildP3);
                Assert.IsNull(result?.GetResults().ChildP4);
                Assert.IsTrue(diagnostics.HasWarnings);
            }
            catch (IOCCException iex)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldCreateTreeForMultipleProfiles()
        {
            IOCCDiagnostics diagnostics = null;
            try
            {
                var sic = Utils.CreateIOCCinAssembly(
                    "ProfileTestData", "ComplexProfile");
                var result1 =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics, new[] { "P1", "P2", "P3" }) as
                        IResultGetter;
                var result2 =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics, new[] { "P1", "P4" }) as
                        IResultGetter;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNotNull(result1?.GetResults().ChildP2);
                Assert.IsNotNull(result1?.GetResults().ChildP3);
                Assert.IsNull(result1?.GetResults().ChildP4);
                Assert.IsTrue(diagnostics.HasWarnings);
            }
            catch (IOCCException iex)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
        }
    }
}