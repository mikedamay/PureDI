using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class DeepHierarchy : IResultGetter
    {
        private Level1X level1;

        [Constructor]
        public DeepHierarchy(
          [BeanReference]Level1X level1)
        {
            this.level1 = level1;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1 = level1;
            return eo;
        }
    }
    [Bean]
    public class Level1X : IResultGetter
    {
        private Level2X level2;
        [Constructor]
        public Level1X(
          [BeanReference]Level2X level2)
        {
            this.level2 = level2;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level2 = level2;
            return eo;
        }
    }
    [Bean]
    public class Level2X
    {
    }
}