using com.TheDisappointedProgrammer.IOCC;
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
                PDependencyInjector sic = Utils.CreateIOCCinAssembly("BadClientTestData", "BadConstructor");
                sic.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadConstructor");
                Assert.Fail();
            }
            catch (IOCCException iex)
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
                PDependencyInjector sic = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactory");
                (var rootBean, var InjectionState) = sic.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadFactory");
                Assert.Fail();
            }
            catch (IOCCException iex)
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
                PDependencyInjector sic = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactoryForParam");
                (var rootBean, var InjectionState) = sic.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadFactoryForParam");
                Assert.Fail();
            }
            catch (IOCCException iex)
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