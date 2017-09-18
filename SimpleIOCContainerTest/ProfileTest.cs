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
            catch (IOCCException)
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
            catch (IOCCException)
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
            catch (IOCCException)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldSelectBestCandidate()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(new string[] { "dobest" });
            sic.SetAssemblies(assembly.GetName().Name);
            IResultGetter result = sic.CreateAndInjectDependencies(
                "IOCCTest.ProfileTestData.BestCandidate", out var diagnostics) as IResultGetter;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(42, result?.GetResults().Val);
        }
        // the following ensures a different branch is taken in some tricky logic
        [TestMethod]
        public void ShouldSelectBestCandidateNewest()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(new string[] { "thirdbest" });
            sic.SetAssemblies(assembly.GetName().Name);
            IResultGetter result = sic.CreateAndInjectDependencies(
                "IOCCTest.ProfileTestData.BestCandidate", out var diagnostics) as IResultGetter;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(33, result?.GetResults().Val);
        }
        [TestMethod]
        public void ShouldSelectBestCandidateWithoutProfile()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer();
            sic.SetAssemblies(assembly.GetName().Name);
            IResultGetter result = sic.CreateAndInjectDependencies(
                "IOCCTest.ProfileTestData.BestCandidate", out var diagnostics) as IResultGetter;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(24, result?.GetResults().Val);
        }
    }
}