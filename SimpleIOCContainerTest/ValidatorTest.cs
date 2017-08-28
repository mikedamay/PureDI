using System.Collections.Generic;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void ShouldWarnIfUnreachableRefereneFromStatic()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ValidatorTestData.Unreachable.cs");
            var diags = new DiagnosticBuilder().Diagnostics;
            new BeanValidator().ValidateAssemblies(new List<Assembly> { assembly }, diags);
            Assert.AreEqual(1, diags.Groups["UnreachableReference"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldWarnIfUnreachableRefereneFromNonBean()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ValidatorTestData.NonBeanNonStatic.cs");
            var diags = new DiagnosticBuilder().Diagnostics;
            new BeanValidator().ValidateAssemblies(new List<Assembly> { assembly }, diags);
            Assert.AreEqual(1, diags.Groups["UnreachableReference"].Occurrences.Count);
        }
        [TestMethod]
        public void ShouldWarnIfUnreachableRefereneWithMultipleAttributes()
        {
            Assembly assembly = CreateAssembly($"{TestResourcePrefix}.ValidatorTestData.MultipleAttributes.cs");
            var diags = new DiagnosticBuilder().Diagnostics;
            new BeanValidator().ValidateAssemblies(new List<Assembly> { assembly }, diags);
            Assert.AreEqual(1, diags.Groups["UnreachableReference"].Occurrences.Count);
        }

    }
}