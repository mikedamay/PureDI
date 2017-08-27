using System;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for DifficultTypesTest
    /// </summary>
    [TestClass]
    public class DifficultTypesTest
    {

        [TestMethod]
        public void ShouldCreateTreeWithReadOnlyFields()
        {
            ReadOnlyFields rof = null;
            try
            {
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependencies<ReadOnlyFields>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(rof);
            Assert.IsNotNull(rof?.GetResults().Field);
        }
        [TestMethod]
        public void ShouldCreateTreeWithAlreadyInitializedFields()
        {
            AlreadyInitialized rof = null;
            try
            {
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependencies<AlreadyInitialized>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(rof);
            Assert.IsNotNull(rof?.GetResults().Field);
        }
        [TestMethod]
        public void ShouldCreateTreeWithProperties()
        {
            MyProps props = null;
            try
            {
                props = SimpleIOCContainer.Instance.CreateAndInjectDependencies<MyProps>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
            Assert.IsNotNull(props);
            Assert.IsNotNull(props?.GetResults().MyProp);
        }
        [TestMethod]
        public void ShouldCreateTreeWithAutoProperties()
        {
            MyAutoProp props = null;
            try
            {
                props = SimpleIOCContainer.Instance.CreateAndInjectDependencies<MyAutoProp>(out IOCCDiagnostics diags);
                Assert.IsTrue(diags.HasWarnings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldWarnOfDynamicReference()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "Dynamic");
            dynamic diag = diagnostics.Groups["MissingBean"].Occurrences[0];
            Assert.AreEqual("anotherDynamic", diag.MemberName);
            Assert.AreEqual("IOCCTest.DifficultTypeTestData.Dynamic", diag.Bean);
            Assert.IsTrue(diagnostics.HasWarnings);
        }
    }
}
