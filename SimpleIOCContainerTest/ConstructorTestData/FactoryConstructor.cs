using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class FactoryConstructor : IResultGetter
    {
        [IOCCBeanReference]
        private Level1 level1;

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
          [IOCCBeanReference(Factory=typeof(Level2Factory))]Level2 level2)
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
    public class Level2Factory : IOCCFactory
    {
        private Level2 level2;

        [IOCCConstructor]
        public Level2Factory(
          [IOCCBeanReference]Level2 level2)
        {
            this.level2 = level2;
        }
        public object Execute(BeanFactoryArgs args)
        {
            return level2;
        }
    }
    [IOCCBean]
    public class Level2
    {
    }
}