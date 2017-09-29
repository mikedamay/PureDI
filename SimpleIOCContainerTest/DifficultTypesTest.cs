using System;
using System.Collections.Generic;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.DifficultTypeTestData;
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
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<ReadOnlyFields>();
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
                rof = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<AlreadyInitialized>();
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
                props = SimpleIOCContainer.Instance.CreateAndInjectDependenciesSimple<MyProps>();
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
                IOCCDiagnostics.Group grp = diags.Groups["ReadOnlyProperty"];
                Assert.AreEqual(1, grp.Occurrences.Count);
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
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual("anotherDynamic", diag.MemberName);
            Assert.AreEqual("IOCCTest.DifficultTypeTestData.Dynamic", diag.Bean);
        }
        [TestMethod]
        public void ShouldWarnOfTuple()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "Tuple");
            Assert.IsNull(result.GetResults().Stuff.Item1);
            Assert.IsTrue(diagnostics.HasWarnings);
        }
        [TestMethod]
        public void ShouldWarnOfInvalidTypes()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "InvalidTypes");
            Assert.IsTrue(diagnostics.HasWarnings);
            dynamic diag0 = diagnostics.Groups["MissingBean"].Occurrences[0];
            Assert.AreEqual("someArray", diag0.MemberName);
            dynamic diag1 = diagnostics.Groups["MissingBean"].Occurrences[1];
            Assert.AreEqual("someObject", diag1.MemberName);
        }

        [TestMethod]
        public void ShouldWarnIfRootIsStatic()
        {
            try
            {
                (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                    "DifficultTypeTestData", "StaticClass");
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.Fail();
            }
            catch (IOCCException iex)
            {
                System.Diagnostics.Debug.WriteLine(iex.Diagnostics);
                Assert.IsTrue(iex.Diagnostics.HasWarnings);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void ShouldWarnOfNullable()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "Nullable");
            Assert.IsTrue(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldWarnOfStaticConstructor()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "StaticConstructor");
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual(1, diagnostics.Groups["MissingNoArgConstructor"].Occurrences.Count);
            
        }
        [TestMethod]
        public void ShouldCreateTreeForPrivateClass()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "PrivateClass");
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.IsNotNull(result.GetResults().Inner);

        }
        [TestMethod]
        public void ShouldCreateTreeForAttributeBean()
        {
            // couldn't create assembly for this
            var result = new SimpleIOCContainer()
              .CreateAndInjectDependencies<AttributeAsBean>(
              out var diagnostics) as IResultGetter;
            Assert.IsNull( result.GetResults().SomeOtherValue);
        }
        [TestMethod]
        public void ShouldCreateTreeForNoGetterProperties()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "DifficultTypeTestData", "NoGetter");
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
            Assert.IsNotNull(result.GetResults().Prop);

        }
    }
}
