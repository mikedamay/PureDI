using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.TestData
{
    [Bean(OS=PDependencyInjector.OS.Any)]
    public class CrossPlatform : IResultGetter
    {
        [BeanReference] private Windows windows = null;
        [BeanReference] private Linux linux = null;
        [BeanReference] private Macos macos = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Windows = windows;
            eo.Linux = linux;
            eo.Macos = macos;
            return eo;
        }

    }
    [Bean(OS= PDependencyInjector.OS.MacOS)]
    internal class Macos
    {
    }
    [Bean(OS= PDependencyInjector.OS.Linux)]
    internal class Linux
    {
    }
    [Bean(OS= PDependencyInjector.OS.Windows)]
    internal class Windows
    {
    }
}