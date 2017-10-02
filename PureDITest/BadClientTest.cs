﻿using PureDI;
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
                PDependencyInjector pdi = Utils.CreateIOCCinAssembly("BadClientTestData", "BadConstructor");
                pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadConstructor");
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
                PDependencyInjector pdi = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactory");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadFactory");
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
                PDependencyInjector pdi = Utils.CreateIOCCinAssembly("BadClientTestData", "BadFactoryForParam");
                (var rootBean, var InjectionState) = pdi.CreateAndInjectDependencies("IOCCTest.BadClientTestData.BadFactoryForParam");
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