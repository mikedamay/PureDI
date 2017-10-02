using System;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class CheckArgumentsTest
    {
        //[Ignore]
        //[TestMethod]
        public void SHouldThrowExceptionIfNullAssemblies()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    //sic.SetAssemblies(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: PDependencyInjector.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependencies<CheckArgumentsTest>(rootBeanName: PDependencyInjector.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullType()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependenciesWithString(null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullBeanName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependenciesWithString("CheckArgumentsTest", rootBeanName: null);
                });

        }

        [TestMethod]
        public void ShouldThrowExceptionForNullConstructorName3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependenciesWithString("CheckArgumentsTest", rootBeanName: PDependencyInjector.DEFAULT_BEAN_NAME, rootConstructorName: null
                    );
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForMissingType()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector();
                    sic.CreateAndInjectDependencies(null);
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector(Profiles: new[] { (string)null });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile2()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector(Profiles: new[] { "" });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile3()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector(Profiles: new[] { " " });
                });

        }
        [TestMethod]
        public void ShouldThrowExceptionForEmptyProfile4()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () =>
                {
                    var sic = new PDependencyInjector(Profiles: new[] { "goodstuff", null });
                });

        }
    }
}