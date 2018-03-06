using System.Dynamic;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.TestCode
{
    // I don't know why I decided on this pattern - maybe just looking for another way to
    // exercise dynamics
    [Ignore]
    public interface IResultGetter
    {
        dynamic GetResults();
    }

    [Bean]
    public class DeepHierahy : IResultGetter
    {
        [BeanReference]
        private Level2a level2a = null;
        [BeanReference]
        private Level2b level2b = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
        }
    }
    [Bean]
    internal class Level2b : IResultGetter
    {
        [BeanReference]
        private Level2b3a level2b3a = null;
        [BeanReference]
        private Level2b3b level2b3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2b3a = level2b3a;
            eo.Level2b3b = level2b3b;
            return eo;
        }
    }

    [Bean]
    internal class Level2b3a
    {
    }

    [Bean]
    internal class Level2b3b
    {
    }

    [Bean]
    internal class Level2a : IResultGetter
    {
        [BeanReference]
        private Level2a3a level2a3a = null;
        [BeanReference]
        private Level2a3b level2a3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a3a = level2a3a;
            eo.Level2a3b = level2a3b;
            return eo;
        }
    }
    [Bean]
    internal class Level2a3a
    {
    }
    [Bean]
    internal class Level2a3b
    {
    }

}