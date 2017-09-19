using System;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class CheckArgumentsTest
    {
        [Ignore]
        [TestMethod]
        public void SHouldThrowExceptionIfNullAssemblies()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    //sic.SetAssemblies(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(out var diags, rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(out var diags, rootBeanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(beanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(beanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullType()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies(null, out var diags);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies("CheckArgumentsTest", out var diags, rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies("CheckArgumentsTest", out var diags, rootBeanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }
    }
}