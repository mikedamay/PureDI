using System.Reflection;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("BadClientTestData", "BadConstructor");
                pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadConstructor"
                  , assemblies: new Assembly[] { assembly});
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
                (DependencyInjector pdi, Assembly assembly) =
                    Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactory");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies(
                    "IOCCTest.BadClientTestData.BadFactory"
                    , assemblies: new Assembly[] { assembly});
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
                (DependencyInjector pdi, Assembly assembly) = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactoryForParam");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies(
                  "IOCCTest.BadClientTestData.BadFactoryForParam", assemblies: new Assembly[]{ assembly});
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