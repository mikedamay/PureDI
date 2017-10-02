using System;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IOCCTest.TestCode;

namespace IOCCTest
{
    [TestClass]
    public class MultipleCallsTest
    {
        [TestMethod]
        public void ShouldExtendTreeWhenMoreCallsAreMode()
        {
            PDependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Simple");
            (object simpleBean, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Simple");
            (object furtherBean, InjectionState injectionState2) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Further", injectionState);
            Assert.AreEqual(simpleBean, ((IResultGetter)furtherBean)?.GetResults().Simple );
        }

        [TestMethod]
        public void ShouldRejectAttemptToCreateTreeForASecondTimeWithoutState()
        {
            //PDependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Fails");
            PDependencyInjector pdi = new PDependencyInjector(Assemblies: new[] { this.GetType().Assembly});
                    // remote assembly refuses to work (Bad IL Format) in this test despite
                    // being identical to the one above
            Assert.ThrowsException<ArgumentException>(() =>
                {
                    (object simpleBean, InjectionState injectionState) 
                        = pdi.CreateAndInjectDependencies(
                        "IOCCTest.MultipleCallsTestData.Fails");
                    (object furtherBean, InjectionState injectionState2) 
                        = pdi.CreateAndInjectDependencies(
                        "IOCCTest.MultipleCallsTestData.FurtherFails");
                }
            );
        }

        [TestMethod]
        public void ShouldCreateTreesWithMultipleCallsWithEmptyState()
        {
            PDependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Empty");
            (object empty1, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.Empty");
            (object empty2, InjectionState @is2) = pdi.CreateAndInjectDependencies(
                "IOCCTest.MultipleCallsTestData.Empty", InjectionState.Empty);
            Assert.AreNotEqual(empty1, empty2);
        }
        [TestMethod]
        public void ShouldCreateTreesWithRecursingFactories()
        {
            PDependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "SimpleFactory");
            (object factory1, InjectionState injectionState) 
              = pdi.CreateAndInjectDependencies(
              "IOCCTest.MultipleCallsTestData.SimpleFactory");
            Assert.IsNotNull( ((IResultGetter)factory1)?.GetResults().SimpleChild);
        }

        [TestMethod]
        public void ShouldCreateTreeWithAComplexArrangementOfFactories()
        {
            PDependencyInjector pdi = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "ComplexFactory");
            (object factory1, InjectionState injectionState)
                = pdi.CreateAndInjectDependencies(
                    "IOCCTest.MultipleCallsTestData.ComplexFactory");
            IResultGetter result = factory1 as IResultGetter;
            ;
            Assert.IsNotNull(result.GetResults()?.ChildOne);
            Assert.IsNotNull(result.GetResults()?.ChildTwo);
            Assert.IsNotNull(((IResultGetter)result.GetResults()?.ChildOne)?.GetResults().ChildTwo);
            Assert.AreEqual(result?.GetResults().ChildTwo
              , ((IResultGetter)result.GetResults()?.ChildOne)?.GetResults().ChildTwo);
        }
    }
}