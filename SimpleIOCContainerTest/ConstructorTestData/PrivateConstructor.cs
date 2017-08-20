using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class PrivateConstructor : IResultGetter
    {
        private IntHolderL intHolder;
        [IOCCConstructor]
        private PrivateConstructor(
          [IOCCBeanReference]IntHolderL intHolder
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

    [IOCCBean]
    public class IntHolderL
    {
        public readonly int heldValue = 42;
    }
}