using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class MultipleConstructors : IResultGetter
    {
        [IOCCBeanReference(ConstructorName = "level2a")]
        private Level1 level1a;
        [IOCCBeanReference(ConstructorName = "level2b")]
        private Level1 level1b;

 

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1a = level1a;
            eo.Level1b = level1b;
            return eo;
        }
    }
    [IOCCBean]
    public class Level1 : IResultGetter
    {
        private Level2a level2a;
        private Level2b level2b;
        [IOCCConstructor(Name="level2a")]
        public Level1(
          [IOCCBeanReference]Level2a level2a)
        {
            this.level2a = level2a;
        }
        [IOCCConstructor(Name="level2b")]
        public Level1(
          [IOCCBeanReference]Level2b level2b)
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
    [IOCCBean]
    public class Level2a
    {
    }
    [IOCCBean]
    public class Level2b
    {
    }
}