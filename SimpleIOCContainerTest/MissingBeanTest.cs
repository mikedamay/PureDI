using System;
using System.Text;
using System.Collections.Generic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MissingBeanTest
    {
        [TestMethod]
        public void ShouldWarnIfTypeMissing()
        {
            MissingType mt 
              = new SimpleIOCContainer().CreateAndInjectDependencies<MissingType>(
              out IOCCDiagnostics diags);
            Assert.IsTrue(diags.HasWarnings);
            dynamic diagnostic = diags.Groups["MissingBean"]?.Occurrences[0];
            Assert.IsNotNull(diagnostic);
            Assert.AreEqual("IOCCTest.TestCode.MissingType", diagnostic?.Bean);
            Assert.AreEqual("System.Int32", diagnostic.MemberType);
            Assert.AreEqual("ii", diagnostic.MemberName);
            Assert.AreEqual("", diagnostic.MemberBeanName);
        }

        [TestMethod]
        public void ShouldErrorIfMissingRootType()
        {
            IOCCDiagnostics diags = null;
            Assert.ThrowsException<IOCCException>(() =>
                new SimpleIOCContainer().CreateAndInjectDependencies<int>(out diags));
            Assert.IsTrue(diags.HasWarnings);
            dynamic diagnostic = diags.Groups["MissingRoot"]?.Occurrences[0];
            Assert.AreEqual("System.Int32", diagnostic.BeanType);
            Assert.AreEqual("", diagnostic.BeanName);
        }
    }
}
