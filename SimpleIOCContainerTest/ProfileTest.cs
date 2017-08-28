using System.Reflection;
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
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.SimpleProfile.cs");
                SimpleIOCContainer sic = new SimpleIOCContainer(new string[] {"Simple"});
                sic.SetAssemblies(assembly.GetName().Name);
                 var result =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.SimpleProfile", out diagnostics) as
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
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.ComplexProfile.cs");
                SimpleIOCContainer sic = new SimpleIOCContainer(new string[] { "P1", "P2", "P3" });
                sic.SetAssemblies(assembly.GetName().Name);
                var result =
                    sic.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics) as
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
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.ComplexProfile.cs");
                SimpleIOCContainer sic1 = new SimpleIOCContainer(new string[] { "P1", "P2", "P3" });
                sic1.SetAssemblies(assembly.GetName().Name);
                SimpleIOCContainer sic2 = new SimpleIOCContainer(new string[] { "P1", "P4" });
                sic2.SetAssemblies(assembly.GetName().Name);
                var result1 =
                    sic1.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics) as
                        IResultGetter;
                var result2 =
                    sic2.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile", out diagnostics) as
                        IResultGetter;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNotNull(result1?.GetResults().ChildP2);
                Assert.IsNotNull(result1?.GetResults().ChildP3);
                Assert.IsNull(result1?.GetResults().ChildP4);
                Assert.IsNull(result2?.GetResults().ChildP2);
                Assert.IsNull(result2?.GetResults().ChildP3);
                Assert.IsNotNull(result2?.GetResults().ChildP4);
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