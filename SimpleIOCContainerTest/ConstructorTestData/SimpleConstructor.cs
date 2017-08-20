using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class SimpleConstructor : IResultGetter
    {
        private IntHolderM intHolder;
        [IOCCConstructor]
        public SimpleConstructor(
          [IOCCBeanReference]IntHolderM intHolder
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
    public class IntHolderM
    {
        public readonly int heldValue = 42;
    }
}