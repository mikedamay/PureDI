using System;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class CheckArgumentsTest
    {
        [TestMethod]
        public void SHouldThrowExceptionIfNullAssemblies()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.SetAssemblies(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullProfile()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(out var diags, null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(out var diags
                        , SimpleIOCContainer.DEFAULT_PROFILE, null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(out var diags
                        , SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.DEFAULT_BEAN_NAME, null
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullProfile2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(
                        SimpleIOCContainer.DEFAULT_PROFILE, null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(
                        SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.DEFAULT_BEAN_NAME, null
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
        public void ShouldThrowExceptionForNullProfile3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies("CheckArgumentsTest", out var diags, null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies("CheckArgumentsTest", out var diags
                        , SimpleIOCContainer.DEFAULT_PROFILE, null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies("CheckArgumentsTest", out var diags
                        , SimpleIOCContainer.DEFAULT_PROFILE, SimpleIOCContainer.DEFAULT_BEAN_NAME, null
                    );
                });

        }
    }
}