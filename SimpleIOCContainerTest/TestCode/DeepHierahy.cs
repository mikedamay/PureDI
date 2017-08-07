using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    public class DeepHierahy : IResultGetter
    {
        [IOCCInjectedDependency]
        private Level2a level2a;
        [IOCCInjectedDependency]
        private Level2b level2b;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
        }
    }
    public interface IResultGetter
    {
        dynamic GetResults();
    }

    [IOCCDependency]
    internal class Level2b : IResultGetter
    {
        [IOCCInjectedDependency]
        private Level2b3a level2b3a;
        [IOCCInjectedDependency]
        private Level2b3b level2b3b;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2b3a = level2b3a;
            eo.Level2b3b = level2b3b;
            return eo;
        }
    }

    [IOCCDependency]
    internal class Level2b3a
    {
    }

    [IOCCDependency]
    internal class Level2b3b
    {
    }

    [IOCCDependency]
    internal class Level2a : IResultGetter
    {
        [IOCCInjectedDependency]
        private Level2a3a level2a3a;
        [IOCCInjectedDependency]
        private Level2a3b level2a3b;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a3a = level2a3a;
            eo.Level2a3b = level2a3b;
            return eo;
        }
    }
    [IOCCDependency]
    internal class Level2a3a
    {
    }
    [IOCCDependency]
    internal class Level2a3b
    {
    }

}