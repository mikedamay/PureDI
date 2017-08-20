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
        private Level1J level1;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1 = level1;
            return eo;
        }
    }
    [IOCCBean]
    public class Level1J : IResultGetter
    {
        private Level2J level2;
        [IOCCConstructor]
        public Level1J(
          [IOCCBeanReference(Factory=typeof(Level2JFactory))]Level2J level2)
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
    public class Level2JFactory : IOCCFactory
    {
        private Level2J level2;

        [IOCCConstructor]
        public Level2JFactory(
          [IOCCBeanReference]Level2J level2)
        {
            this.level2 = level2;
        }
        public object Execute(BeanFactoryArgs args)
        {
            return level2;
        }
    }
    [IOCCBean]
    public class Level2J
    {
    }
}