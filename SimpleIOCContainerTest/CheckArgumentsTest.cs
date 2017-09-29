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
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
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
                    sic.CreateAndInjectDependenciesSimple<CheckArgumentsTest>(beanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependenciesSimple<CheckArgumentsTest>(beanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
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
                    sic.CreateAndInjectDependenciesWithString(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependenciesWithString("CheckArgumentsTest", rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer();
                    sic.CreateAndInjectDependenciesWithString("CheckArgumentsTest", rootBeanName: SimpleIOCContainer.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer(Profiles: new[] { (string)null });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer(Profiles: new[] { "" });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer(Profiles: new[] { " " });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile4()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new SimpleIOCContainer(Profiles: new[] { "goodstuff", null });
                });

        }
    }
}