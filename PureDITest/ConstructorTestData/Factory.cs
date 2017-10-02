using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class Factory : IResultGetter
    {
        [BeanReference]
        private Level1H level1;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1 = level1;
            return eo;
        }
    }
    [Bean]
    public class Level1H : IResultGetter
    {
        private Level2H level2;
        [Constructor]
        public Level1H(
          [BeanReference(Factory=typeof(Level2Factory))]Level2H level2)
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
    public class Level2Factory : IFactory
    {
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return (new Level2H(), injectionState);
        }
    }
    [Bean]
    public class Level2H
    {
    }
}