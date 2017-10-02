using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class SimpleConstructor : IResultGetter
    {
        private IntHolderM intHolder;
        [Constructor]
        public SimpleConstructor(
          [BeanReference]IntHolderM intHolder
          )
        {
            this.intHolder = intHolder;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = intHolder.heldValue;
            return eo;
        }
    }

    [Bean]
    public class IntHolderM
    {
        public readonly int heldValue = 42;
    }
}