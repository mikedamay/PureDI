using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class PrivateConstructor : IResultGetter
    {
        private IntHolderL intHolder;
        [Constructor]
        private PrivateConstructor(
          [BeanReference]IntHolderL intHolder
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
    public class IntHolderL
    {
        public readonly int heldValue = 42;
    }
}