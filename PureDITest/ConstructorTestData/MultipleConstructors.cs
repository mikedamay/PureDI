using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class MultipleConstructors : IResultGetter
    {
        [BeanReference(ConstructorName = "level2a")]
        private Level1K level1a = null;
        [BeanReference(ConstructorName = "level2b")]
        private Level1K level1b = null;

 

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1a = level1a;
            eo.Level1b = level1b;
            return eo;
        }
    }
    [Bean]
    public class Level1K
    {
        public Level2Ka Level2a;
        public Level2Kb Level2b;
        [Constructor(Name="level2a")]
        public Level1K(
          [BeanReference]Level2Ka level2a)
        {
            this.Level2a = level2a;
        }
        [Constructor(Name="level2b")]
        public Level1K(
          [BeanReference]Level2Kb level2b)
        {
            this.Level2b = level2b;
        }

    }
    [Bean]
    public class Level2Ka
    {
    }
    [Bean]
    public class Level2Kb
    {
    }
}