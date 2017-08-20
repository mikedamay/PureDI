using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class DeepHierarchy : IResultGetter
    {
        private Level1X level1;

        [IOCCConstructor]
        public DeepHierarchy(
          [IOCCBeanReference]Level1X level1)
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
    [IOCCBean]
    public class Level1X : IResultGetter
    {
        private Level2X level2;
        [IOCCConstructor]
        public Level1X(
          [IOCCBeanReference]Level2X level2)
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
    [IOCCBean]
    public class Level2X
    {
    }
}