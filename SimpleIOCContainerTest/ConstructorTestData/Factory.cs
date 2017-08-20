using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class Factory : IResultGetter
    {
        [IOCCBeanReference]
        private Level1H level1;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Level1 = level1;
            return eo;
        }
    }
    [IOCCBean]
    public class Level1H : IResultGetter
    {
        private Level2H level2;
        [IOCCConstructor]
        public Level1H(
          [IOCCBeanReference(Factory=typeof(Level2Factory))]Level2H level2)
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
        public object Execute(BeanFactoryArgs args)
        {
            return new Level2H();
        }
    }
    [IOCCBean]
    public class Level2H
    {
    }
}