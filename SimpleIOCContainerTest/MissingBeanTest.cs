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
            (MissingType mt, InjectionState InjectionState) 
              = new SimpleIOCContainer().CreateAndInjectDependencies<MissingType>();
            IOCCDiagnostics diags = InjectionState.Diagnostics;
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
            InjectionState injectionState = null;
            int ii;
            try
            {
                (ii, injectionState) = new SimpleIOCContainer().CreateAndInjectDependencies<int>();
                Assert.Fail();
            }
            catch (IOCCException iex)
            {
                diags = iex.Diagnostics;
                Assert.IsTrue(diags.HasWarnings);
                dynamic diagnostic = diags.Groups["MissingRoot"]?.Occurrences[0];
                Assert.AreEqual("System.Int32", diagnostic.BeanType);
                Assert.AreEqual("", diagnostic.BeanName);

            }
            catch (Exception ex)
            {
                var dx = ex;
                Assert.Fail();
            }
        }
    }
}
