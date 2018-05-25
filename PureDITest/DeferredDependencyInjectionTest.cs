using System.Reflection;
using IOCCTest.DeferredTestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI;

namespace IOCCTest
{
    [TestClass]
    public class DeferredDependencyInjectionTest
    {
        [TestMethod]
        public void ShouldNotInitializeMembersWhenDDIFlagSet()
        {
            var pdi = new PDependencyInjector();
            (Simple simple, InjectionState @is) = pdi.CreateAndInjectDependencies<Simple>(deferredDependencyInjection: true);
            Assert.IsNull(simple?.SimpleChild);
            Assert.IsNull(simple?.NotSimpleChild);
            Assert.AreEqual(1, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
            (_, @is) = pdi.CreateAndInjectDependencies(simple, @is);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldNotInitializeMembersWhenDDIFlagSet2()
        {
            var pdi = new PDependencyInjector();
            (object oSimple, InjectionState @is) = pdi.CreateAndInjectDependencies(
              typeof(Simple), null, null, deferDependencyInjection: true);
            Simple simple = oSimple as Simple;
            Assert.IsNull(simple?.SimpleChild);
            Assert.IsNull(simple?.NotSimpleChild);
            Assert.AreEqual(1, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
            (_, @is) = pdi.CreateAndInjectDependencies(simple, @is);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldNotInitializeMembersWhenDDIFlagSet3()
        {
            var pdi = new PDependencyInjector();
            (object oSimple, InjectionState @is) = pdi.CreateAndInjectDependencies(
              typeof(Simple).ToString()
              ,assemblies: new Assembly[] { this.GetType().Assembly} 
              ,deferDependencyInjection: true);
            Simple simple = oSimple as Simple;
            Assert.IsNull(simple?.SimpleChild);
            Assert.IsNull(simple?.NotSimpleChild);
            Assert.AreEqual(1, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
            (_, @is) = pdi.CreateAndInjectDependencies(simple, @is);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldNotInitializeMembersWhenDDIFlagSetForRootObject()
        {
            var simple = new Simple();
            var pdi = new PDependencyInjector();
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
            var pdi = new PDependencyInjector();
            (_, InjectionState @is) = pdi.CreateAndInjectDependencies(simple, deferDepedencyInjection: false);
            Assert.IsNotNull(simple?.SimpleChild);
            Assert.IsNotNull(simple?.NotSimpleChild);
            Assert.AreEqual(0, @is.Diagnostics.Groups["IncompleteInjections"].Occurrences.Count);
        }
    }
}