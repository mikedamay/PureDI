using PureDI;
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
            DependencyInjector pdi = new DependencyInjector();
            Assert.IsFalse(pdi.HasBeenDefinition(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
                    // the assembly was not included - a bit of a gotcha
            Assert.IsNull(pdi.GetBean(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsFalse(pdi.IsBeanInstantiated(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
            pdi.CreateAndInjectDependencies<Beaner>();
            Assert.IsTrue(pdi.HasBeenDefinition(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsNotNull(pdi.GetBean(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsTrue(pdi.IsBeanInstantiated(typeof(Beaner), DependencyInjector.DEFAULT_BEAN_NAME));
            Assert.IsFalse(pdi.HasBeenDefinition(typeof(NonBeaner), DependencyInjector.DEFAULT_BEAN_NAME));
        }

    }
}