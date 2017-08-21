using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class MultipleConstructors : IResultGetter
    {
        [BeanReference(ConstructorName = "level2a")]
        private Level1K level1a;
        [BeanReference(ConstructorName = "level2b")]
        private Level1K level1b;

 

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1a = level1a;
            eo.Level1b = level1b;
            return eo;
        }
    }
    [Bean]
    public class Level1K : IResultGetter
    {
        private Level2Ka level2a;
        private Level2Kb level2b;
        [Constructor(Name="level2a")]
        public Level1K(
          [BeanReference]Level2Ka level2a)
        {
            this.level2a = level2a;
        }
        [Constructor(Name="level2b")]
        public Level1K(
          [BeanReference]Level2Kb level2b)
        {
            this.level2b = level2b;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2a = level2a;
            eo.Level2b = level2b;
            return eo;
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