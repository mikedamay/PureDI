using System.Dynamic;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class CyclicalDependencyWithNamedConstructor : IResultGetter
    {
        private Level1 level1;

        [Constructor]
        public CyclicalDependencyWithNamedConstructor(
            [BeanReference(ConstructorName = "MyConstructor")]Level1 level1)
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
    public class Level1 : IResultGetter
    {
        private Level2 level2;
        [Constructor(Name="MyConstructor")]
        public Level1(
            [BeanReference]Level2 level2)
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
    public class Level2
    {
        [Constructor]
        public Level2(
            [BeanReference(ConstructorName="MyConstructor")] Level1 level1)
        {
            
        }
    }
}