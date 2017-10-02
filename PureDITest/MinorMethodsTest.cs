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
            PDependencyInjector pdi = new PDependencyInjector();
            Assert.IsFalse(pdi.HasBeenDefinition(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
                    // the assembly was not included - a bit of a gotcha
            Assert.IsNull(pdi.GetBean(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsFalse(pdi.IsBeanInstantiated(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
            pdi.CreateAndInjectDependencies<Beaner>();
            Assert.IsTrue(pdi.HasBeenDefinition(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsNotNull(pdi.GetBean(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsTrue(pdi.IsBeanInstantiated(typeof(Beaner), PDependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsFalse(pdi.HasBeenDefinition(typeof(NonBeaner), PDependencyInjector.DEFAULT_BEAN_NAME));
        }

    }
}