using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class ContainerAsBeanTest
    {
        [IOCCBean]
        private class TrivialBean
        {
            [IOCCBeanReference]
            public SimpleIOCContainer child;
        }

        [IOCCBean]
        private class Child
        {
            
        }
        [TestMethod]
        public void ShouldIncludeContainerOnlyOnceInTree()
        {
            try
            {
                SimpleIOCContainer sic = new SimpleIOCContainer();
                ContainerAsBeanTest.TrivialBean tb 
                  = sic.GetOrCreateObjectTree<ContainerAsBeanTest.TrivialBean>();
                Assert.AreEqual(sic, tb.child);
            } finally { }
            /*
            catch (IOCCException e)
            {
                Console.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(e.Diagnostics);
                Assert.Fail();
            }*/
        }

}
}