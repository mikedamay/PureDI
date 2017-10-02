using com.TheDisappointedProgrammer.IOCC;
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
            IOCCDiagnostics diagnostics = null;
            try
            {
                var sic = CreateIOCCinAssembly("EntryPointTestData", "RootInterface");
                sic.CreateAndInjectDependenciesWithString("xxx");
                Assert.Fail();
            }
            catch (IOCCException iex)
            {
                System.Diagnostics.Debug.WriteLine(diagnostics);
                Assert.IsTrue(iex.Diagnostics.HasWarnings);
            }
        }
    }
}