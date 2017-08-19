using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class CyclicalDependency : IResultGetter
    {
        private Level1 level1;

        [IOCCConstructor]
        public CyclicalDependency(
          [IOCCBeanReference]Level1 level1)
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
    public class Level1 : IResultGetter
    {
        private Level2 level2;
        [IOCCConstructor]
        public Level1(
          [IOCCBeanReference]Level2 level2)
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
    public class Level2
    {
        [IOCCConstructor]
        public Level2(
          [IOCCBeanReference] Level1 level1)
        {
            
        }
    }
}