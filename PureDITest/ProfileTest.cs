using System.Reflection;
using PureDI;
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
            Diagnostics diagnostics = null;
            try
            {
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.SimpleProfile.cs");
                PDependencyInjector pdi = new PDependencyInjector(profiles: new string[] {"Simple"}, assemblies: new[] { assembly });
                //pdi.SetAssemblies(assembly.GetName().Name);
                 (object rootBean, InjectionState injectionState) =
                    pdi.CreateAndInjectDependencies("IOCCTest.ProfileTestData.SimpleProfile");
                var result = rootBean as IResultGetter;
                diagnostics = injectionState.Diagnostics;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNull(result?.GetResults().Child);
                Assert.IsTrue(diagnostics.HasWarnings);
            }
            catch (DIException)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldCreateTreeForComplexProfile()
        {
            Diagnostics diagnostics = null;
            try
            {
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.ComplexProfile.cs");
                PDependencyInjector pdi = new PDependencyInjector(profiles: new string[] { "P1", "P2", "P3" }, assemblies: new[] { assembly });
                //pdi.SetAssemblies(assembly.GetName().Name);
                (object rootBean, InjectionState injectionState) =
                    pdi.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile");
                var result = rootBean as IResultGetter;
                diagnostics = injectionState.Diagnostics;
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsNotNull(result?.GetResults().ChildP2);
                Assert.IsNotNull(result?.GetResults().ChildP3);
                Assert.IsNull(result?.GetResults().ChildP4);
                Assert.IsTrue(diagnostics.HasWarnings);
            }
            catch (DIException)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldCreateTreeForMultipleProfiles()
        {
            Diagnostics diagnostics = null;
            try
            {
                Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.ComplexProfile.cs");
                PDependencyInjector sic1 = new PDependencyInjector(profiles: new string[] { "P1", "P2", "P3" }, assemblies: new[] { assembly });
                //sic1.SetAssemblies(assembly.GetName().Name);
                PDependencyInjector sic2 = new PDependencyInjector(profiles: new string[] { "P1", "P4" }, assemblies : new[] { assembly });
                //sic2.SetAssemblies(assembly.GetName().Name);
                (object rootBean1, InjectionState injectionState) =
                    sic1.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile");
                var result1 = rootBean1 as IResultGetter;
                diagnostics = injectionState.Diagnostics;
                var result2 =
                    sic2.CreateAndInjectDependencies("IOCCTest.ProfileTestData.ComplexProfile").rootBean as
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
            catch (DIException)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShouldSelectBestCandidate()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ProfileTestData.BestCandidate.cs");
            PDependencyInjector pdi = new PDependencyInjector(profiles: new string[] { "dobest" }, assemblies: new[] { assembly });
            //pdi.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = pdi.CreateAndInjectDependencies(
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
            PDependencyInjector pdi = new PDependencyInjector(profiles:new string[] { "thirdbest" }, assemblies: new[] { assembly });
            //pdi.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = pdi.CreateAndInjectDependencies(
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
            PDependencyInjector pdi = new PDependencyInjector(assemblies: new[] { assembly });
            //pdi.SetAssemblies(assembly.GetName().Name);
            (object rootBean, InjectionState injectionState) = pdi.CreateAndInjectDependencies(
                "IOCCTest.ProfileTestData.BestCandidate");
            var result = rootBean as IResultGetter;
            var diagnostics = injectionState.Diagnostics;
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.AreEqual(24, result?.GetResults().Val);
        }
    }
}