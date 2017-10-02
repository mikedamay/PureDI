using System;
using System.IO;
using System.Reflection;
using PureDI;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class FactoryTest
    {
        [TestMethod]
        public void ShouldCreateTreeFromRootAsString()
        {

            (var result, var diagnostics) = CommonFactoryTest("AccessByString");
            Assert.IsNotNull(result);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldBuildTreeWithSimpleFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("SimpleBean");
            Assert.AreEqual(10, result.Abc);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldBuildTreeWithFactoryAndMemberBeans()
        {
            (var result, var diagnostics) = CommonFactoryTest("FactoryWithMemberBeans");
            Assert.IsNotNull(result?.GetResults().Member);
            Assert.IsNotNull(result?.GetResults().Member?.GetResults().SubMember);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldWarnIfFactoryMissing()
        {
            (var result, var diagnostics) = CommonFactoryTest("MissingFactory");
            Assert.IsTrue(diagnostics.HasWarnings);
            Assert.AreEqual(1, diagnostics.Groups["MissingFactory"].Occurrences.Count);
        }

        [TestMethod]
        public void ShouldCreateTreeForFactoryWithGenerics()
        {
            (var result, var diagnostics) = CommonFactoryTest("Generic");
            Assert.IsNotNull(result);
            Assert.IsNotNull(result?.GetResults().MyGeneric);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldWarnOnFactoryMismatch()
        {
            (var result, var diagnostics) = CommonFactoryTest("TypeMismatch");
            Assert.AreEqual(1, diagnostics.Groups["TypeMismatch"].Occurrences.Count);
        }

        [TestMethod]
        public void ShouldWarnIfFactoryExeccuteThrowsExcption()
        {
            try
            {
                (var result, var diagnostics) = CommonFactoryTest("ThrowsException");
                Assert.Fail();
            }
            catch (DIException ex)
            {
                Assert.AreEqual(1, ex.Diagnostics.Groups["FactoryExecutionFailure"].Occurrences.Count);
            }
        }

        [TestMethod]
        public void ShouldCreateTreeForFactoryWithDependencies()
        {
            (var result, var diagnostics) = CommonFactoryTest("FactoryDependencies");
            Assert.AreEqual(17, result?.GetResults().SomeValue);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldCreateTreeForGenericFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("GenericFactory");
            Assert.IsNotNull(result?.GetResults().MyThing);
            Assert.IsNotNull(result?.GetResults().MySecondThing);
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
        }

        [TestMethod]
        public void ShouldWarnOfBadFactory()
        {
            (var result, var diagnostics) = CommonFactoryTest("BadFactory");
            Assert.IsTrue(diagnostics.HasWarnings);
        }

        [TestMethod]
        public void ShouldCreateTreeWithNamedFactory()
        {
            (dynamic result, var diagnostics) = CommonFactoryTest("FactoryWithName");
            Assert.AreEqual(42, result.GetResults().MysteryNumber);
            Assert.IsFalse(diagnostics.HasWarnings);
        }
        private static
            (dynamic result, Diagnostics diagnostics)
            CommonFactoryTest(string className)
        {
            return Utils.CreateAndRunAssembly("FactoryTestData", className);
        }

        public static string GetResource(string resourceName)
        {
            return Utils.GetResource(resourceName);
        }
    }
}