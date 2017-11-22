using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;
using PureDI.Public;

namespace IOCCTest.TestData
{
    [Bean(OS=Os.Any)]
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
    [Bean(OS= Os.MacOs)]
    internal class Macos
    {
    }
    [Bean(OS= Os.Linux)]
    internal class Linux
    {
    }
    [Bean(OS= Os.Windows)]
    internal class Windows
    {
    }
}