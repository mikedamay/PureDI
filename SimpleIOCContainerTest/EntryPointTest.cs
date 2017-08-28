using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IOCCTest.Utils;

namespace IOCCTest
{
    [TestClass]
    public class EntryPointTest
    {
        [TestMethod]
        public void ShouldCreateTreeWithRootInterface()
        {
            (dynamic result, var diagnostics) = Utils.CreateAndRunAssembly(
                "EntryPointTestData", "RootInterface");
            Assert.IsFalse(Falsify(diagnostics.HasWarnings));
    }
}
}