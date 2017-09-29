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
                SimpleIOCContainer sic = new SimpleIOCContainer(Profiles: new string[] {"Simple"}, Assemblies: new[] { assembly });
                //sic.SetAssemblies(assembly.GetName().Name);
                 (object rootBean, InjectionState injectionState) =
                    sic.CreateAndInjectDependenciesWithString("IOCCTest.ProfileTestData.SimpleProfile");
                var result = rootBean as IResultGetter;
                diagnostics = injectionState.Diagnostics;
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
                SimpleIOCContainer sic = new SimpleIOCContainer(Profiles: new string[] { "P1", "P2", "P3" }, Assemblies: new[] { assembly });
                //sic.SetAssemblies(assembly.GetName().Name);
                (object rootBean, InjectionState injectionState) =
                    sic.CreateAndInjectDependenciesWithString("IOCCTest.ProfileTestData.ComplexProfile");
                var result = rootBean as IResultGetter;
                diagnostics = injectionState.Diagnostics;
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
                SimpleIOCContainer sic1 = new SimpleIOCContainer(Profiles: new string[] { "P1", "P2", "P3" }, Assemblies: new[] { assembly });
                //sic1.SetAssemblies(assembly.GetName().Name);
                SimpleIOCContainer sic2 = new SimpleIOCContainer(Profiles: new string[] { "P1", "P4" }, Assemblies : new[] { assembly });
                //sic2.SetAssemblies(assembly.GetName().Name);
                (object rootBean1, InjectionState injectionState) =
                    sic1.CreateAndInjectDependenciesWithString("IOCCTest.ProfileTestData.ComplexProfile");
                var result1 = rootBean1 as IResultGetter;
                diagnostics = injectionState.Diagnostics;
                var result2 =
                    sic2.CreateAndInjectDependenciesWithString("IOCCTest.ProfileTestData.ComplexProfile").rootBean as
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
            SimpleIOCContainer sic = new SimpleIOCContainer(Profiles: new string[] { "dobest" }, Assemblies: new[] { assembly });
            //sic.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = sic.CreateAndInjectDependenciesWithString(
                "IOCCTest.ProfileTestData.BestCandidate");
            var result = rootBean as IResultGetter;
            var diagnostics = injectionState.Diagnostics;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(42, result?.GetResults().Val);
        }
        // the following ensures a different branch is taken in some tricky logic
        [TestMethod]
        public void ShouldSelectBestCandidateNewest()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(Profiles:new string[] { "thirdbest" }, Assemblies: new[] { assembly });
            //sic.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = sic.CreateAndInjectDependenciesWithString(
                "IOCCTest.ProfileTestData.BestCandidate");
            var result = rootBean as IResultGetter;
            var diagnostics = injectionState.Diagnostics;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(33, result?.GetResults().Val);
        }
        [TestMethod]
        public void ShouldSelectBestCandidateWithoutProfile()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            SimpleIOCContainer sic = new SimpleIOCContainer(Assemblies: new[] { assembly });
            //sic.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = sic.CreateAndInjectDependenciesWithString(
                "IOCCTest.ProfileTestData.BestCandidate");
            var result = rootBean as IResultGetter;
            var diagnostics = injectionState.Diagnostics;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(24, result?.GetResults().Val);
        }
    }
}