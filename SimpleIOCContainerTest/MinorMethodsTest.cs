using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.MinorMethodsData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;
namespace IOCCTest
{
    [TestClass]
    public class MinorMethodsTest
    {
        [TestMethod]
        public void ShouldIdentifyAvailableBean()
        {
            SimpleIOCContainer sic = new SimpleIOCContainer();
            Assert.IsFalse(sic.HasBeenDefinition(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
                    // the assembly was not included - a bit of a gotcha
            Assert.IsNull(sic.GetBean(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
            Assert.IsFalse(sic.IsBeanInstantiated(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
            sic.CreateAndInjectDependencies<Beaner>();
            Assert.IsTrue(sic.HasBeenDefinition(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
            Assert.IsNotNull(sic.GetBean(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
            Assert.IsTrue(sic.IsBeanInstantiated(typeof(Beaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
            Assert.IsFalse(sic.HasBeenDefinition(typeof(NonBeaner), SimpleIOCContainer.DEFAULT_BEAN_NAME));
        }

    }
}