using System.Dynamic;
using PureDI;

namespace IOCCTest.TestCode.WithNames
{
    [Ignore]
    public interface IResultGetter
    {
        dynamic GetResults();
    }

    [Bean]
    public class DeepHierahy : IResultGetter
    {
        [BeanReference(Name="level2a")]
        private Level2 level2a = null;
        [BeanReference(Name="level2b")]
        private Level2 level2b = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
        }
    }
    public interface Level2
    {

    }
    [Bean(Name="level2b")]
    internal class Level2b : IResultGetter, Level2
    {
        [BeanReference(Name="level2b3a")]
        private Level2b3a level2b3a = null;
        [BeanReference(Name="level2b3b")]
        private Level2b3b level2b3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2b3a = level2b3a;
            eo.Level2b3b = level2b3b;
            return eo;
        }
    }
    
    [Bean(Name="level2b3a")]
    internal class Level2b3a
    {
    }

    [Bean(Name="level2b3b")]
    internal class Level2b3b
    {
    }

    [Bean(Name = "level2a")]
    internal class Level2a : IResultGetter, Level2
    {
        [BeanReference(Name="level2a3a")]
        private Level2a3a level2a3a = null;
        [BeanReference(Name="level2a3b")]
        private Level2a3b level2a3b = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a3a = level2a3a;
            eo.Level2a3b = level2a3b;
            return eo;
        }
    }
    [Bean(Name="level2a3a")]
    internal class Level2a3a
    {
    }
    [Bean(Name="level2a3b")]
    internal class Level2a3b
    {
    }

}