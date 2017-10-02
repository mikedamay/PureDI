using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class ContainerAsBeanTest
    {
        [Bean]
        private class TrivialBean
        {
            [BeanReference]
            public PDependencyInjector child = null;

        }

        [Bean]
        private class Child
        {
            
        }
        [TestMethod]
        public void ShouldIncludeContainerOnlyOnceInTree()
        {
            try
            {
                PDependencyInjector sic = new PDependencyInjector();
                ContainerAsBeanTest.TrivialBean tb 
                  = sic.CreateAndInjectDependencies<ContainerAsBeanTest.TrivialBean>().rootBean;
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