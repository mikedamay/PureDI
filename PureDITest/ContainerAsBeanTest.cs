using System;
using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureDI.Attributes;

namespace IOCCTest
{
    [TestClass]
    public class ContainerAsBeanTest
    {
        [Bean]
        private class TrivialBean
        {
            [BeanReference]
            public DependencyInjector child = null;

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
                DependencyInjector pdi = new DependencyInjector();
                ContainerAsBeanTest.TrivialBean tb 
                  = pdi.CreateAndInjectDependencies<ContainerAsBeanTest.TrivialBean>().rootBean;
                Assert.AreEqual(pdi, tb.child);
            } finally { }
            /*
            catch (DIException e)
            {
                Console.WriteLine(e);
                System.Diagnostics.Debug.WriteLine(e.Diagnostics);
                Assert.Fail();
            }*/
        }

}
}