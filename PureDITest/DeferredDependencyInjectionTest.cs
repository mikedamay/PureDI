using IOCCTest.DeferredTestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI;

namespace IOCCTest
{
    [TestClass]
    public class DeferredDependencyInjectionTest
    {
        [TestMethod]
        public void ShouldNotInitializeMembersWhenDDIFlagSetForRootObject()
        {
            var simple = new Simple();
            var pdi = new DependencyInjector();
            (_, InjectionState @is) = pdi.CreateAndInjectDependencies(simple, deferDepedencyInjection: true);
            Assert.IsNull(simple?.SimpleChild);
            Assert.IsNull(simple?.NotSimpleChild);
            Assert.AreEqual(1, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
            (_, @is) = pdi.CreateAndInjectDependencies(simple, @is);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldInitializeMembersWhenDDINotFlagSetForRootObject()
        {
            var simple = new Simple();
            var pdi = new DependencyInjector();
            (_, InjectionState @is) = pdi.CreateAndInjectDependencies(simple, deferDepedencyInjection: false);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
    }
}