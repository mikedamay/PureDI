using System.Reflection;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI.Public;

namespace IOCCTest
{
    [TestClass]
    public class BadClientTest
    {
        [TestMethod]
        public void ShouldThrowExceptionOnBadConstructor()
        {
            try
            {
                (PDependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("BadClientTestData", "BadConstructor");
                pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadConstructor"
                  , assemblySpec: new AssemblySpec(assemblies: assembly));
                Assert.Fail();
            }
            catch (DIException iex)
            {
                var ix = iex;
                Assert.IsTrue(true);
            }
            catch (System.Exception ex)
            {
                var x = ex;
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldThrowExceptionOnBadFactory()
        {
            try
            {
                (PDependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactory");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadFactory"
                  , assemblySpec: new AssemblySpec(assemblies: assembly));
                Assert.Fail();
            }
            catch (DIException iex)
            {
                var ix = iex;
                Assert.IsTrue(true);
            }
            catch (System.Exception ex)
            {
                var x = ex;
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldThrowExceptionOnBadFactoryForParam()
        {
            try
            {
                (PDependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactoryForParam");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies(
                  "IOCCTest.BadClientTestData.BadFactoryForParam", assemblySpec: new AssemblySpec(assemblies: assembly));
                Assert.Fail();
            }
            catch (DIException iex)
            {
                var ix = iex;
                Assert.IsTrue(true);
            }
            catch (System.Exception ex)
            {
                var x = ex;
                Assert.Fail();
            }
        }
    }
}