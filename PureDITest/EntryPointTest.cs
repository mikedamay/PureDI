using PureDI;
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

        [TestMethod]
        public void ShouldProvideDiagnosticIfBadTypeString()
        {
            Diagnostics diagnostics = null;
            try
            {
                var pdi = CreateIOCCinAssembly("EntryPointTestData", "RootInterface");
                pdi.CreateAndInjectDependencies("xxx");
                Assert.Fail();
            }
            catch (DIException iex)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsTrue(iex.Diagnostics.HasWarnings);
            }
        }
    }
}