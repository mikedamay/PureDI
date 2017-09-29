using System;
using com.TheDisappointedProgrammer.IOCC;
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
            SimpleIOCContainer sic = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Simple");
            (object simpleBean, InjectionState injectionState) 
              = sic.CreateAndInjectDependenciesWithString(
              "IOCCTest.MultipleCallsTestData.Simple");
            (object furtherBean, InjectionState injectionState2) 
              = sic.CreateAndInjectDependenciesWithString(
              "IOCCTest.MultipleCallsTestData.Further", injectionState);
            Assert.AreEqual(simpleBean, ((IResultGetter)furtherBean)?.GetResults().Simple );
        }

        [TestMethod]
        public void ShouldRejectAttemptToCreateTreeForASecondTimeWithoutState()
        {
            //SimpleIOCContainer sic = Utils.CreateIOCCinAssembly("MultipleCallsTestData", "Fails");
            SimpleIOCContainer sic = new SimpleIOCContainer(Assemblies: new[] { this.GetType().Assembly});
                    // remote assembly refuses to work (Bad IL Format) in this test despite
                    // being identical to the one above
            Assert.ThrowsException<ArgumentException>(() =>
                {
                    (object simpleBean, InjectionState injectionState) 
                        = sic.CreateAndInjectDependenciesWithString(
                        "IOCCTest.MultipleCallsTestData.Fails");
                    (object furtherBean, InjectionState injectionState2) 
                        = sic.CreateAndInjectDependenciesWithString(
                        "IOCCTest.MultipleCallsTestData.FurtherFails");
                }
            );
        }
    }
}