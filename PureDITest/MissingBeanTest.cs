using System;
using System.Text;
using System.Collections.Generic;
using PureDI;
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
              = new PDependencyInjector().CreateAndInjectDependencies<MissingType>();
            Diagnostics diags = InjectionState.Diagnostics;
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
            Diagnostics diags = null;
            InjectionState injectionState = null;
            int ii;
            try
            {
                (ii, injectionState) = new PDependencyInjector().CreateAndInjectDependencies<int>();
                Assert.Fail();
            }
            catch (DIException iex)
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
