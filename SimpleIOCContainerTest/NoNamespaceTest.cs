using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class NoNamespaceTest
    {
        [TestMethod]
        public void ShouldCreateTreeWithNoNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NoNamespaceData", "NoNamespace");
            object rootBean = iocc.CreateAndInjectDependencies("NoNamespace"
              , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsFalse(diagnostics.HasWarnings);

        }
        [TestMethod]
        public void ShouldCreateTreeWithReferenceFromNoNamespaceToNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NoNamespaceData", "ReferenceToNamespacedClass");
            object rootBean = iocc.CreateAndInjectDependencies("ReferenceToNamespacedClass"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsNotNull(((IResultGetter)rootBean).GetResults().Referred);
            Assert.IsFalse(diagnostics.HasWarnings);

        }
        [TestMethod]
        public void ShouldCreateTreeWithReferenceToNoNamespaceFromNamespace()
        {
            var iocc = Utils.CreateIOCCinAssembly("NoNamespaceData", "ReferenceFromNamespacedClass");
            object rootBean = iocc.CreateAndInjectDependencies("IOCCTest.NoNamespaceData.ReferenceFromNamespacedClass"
                , out IOCCDiagnostics diagnostics);
            System.Diagnostics.Debug.WriteLine(diagnostics);
            Assert.IsNotNull(rootBean);
            Assert.IsNotNull(((IResultGetter)rootBean).GetResults().Referred);
            Assert.IsFalse(diagnostics.HasWarnings);

        }
    }
}