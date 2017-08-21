using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class FactoryConstructor : IResultGetter
    {
        [BeanReference]
        private Level1J level1;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1 = level1;
            return eo;
        }
    }
    [Bean]
    public class Level1J : IResultGetter
    {
        private Level2J level2;
        [Constructor]
        public Level1J(
          [BeanReference(Factory=typeof(Level2JFactory))]Level2J level2)
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
    public class Level2JFactory : IFactory
    {
        private Level2J level2;

        [Constructor]
        public Level2JFactory(
          [BeanReference]Level2J level2)
        {
            this.level2 = level2;
        }
        public object Execute(BeanFactoryArgs args)
        {
            return level2;
        }
    }
    [Bean]
    public class Level2J
    {
    }
}