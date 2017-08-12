using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    // I don't know why I decided on this pattern - maybe just looking for another way to
    // exercise dynamics
    public interface IResultGetter
    {
        dynamic GetResults();
    }

    [IOCCBean]
    public class DeepHierahy : IResultGetter
    {
        [IOCCBeanReference]
        private Level2a level2a = null;
        [IOCCBeanReference]
        private Level2b level2b = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
        }
    }
    [IOCCBean]
    internal class Level2b : IResultGetter
    {
        [IOCCBeanReference]
        private Level2b3a level2b3a = null;
        [IOCCBeanReference]
        private Level2b3b level2b3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2b3a = level2b3a;
            eo.Level2b3b = level2b3b;
            return eo;
        }
    }

    [IOCCBean]
    internal class Level2b3a
    {
    }

    [IOCCBean]
    internal class Level2b3b
    {
    }

    [IOCCBean]
    internal class Level2a : IResultGetter
    {
        [IOCCBeanReference]
        private Level2a3a level2a3a = null;
        [IOCCBeanReference]
        private Level2a3b level2a3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a3a = level2a3a;
            eo.Level2a3b = level2a3b;
            return eo;
        }
    }
    [IOCCBean]
    internal class Level2a3a
    {
    }
    [IOCCBean]
    internal class Level2a3b
    {
    }

}