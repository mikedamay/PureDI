using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class UnmarkedParameter : IResultGetter
    {
        private IntHolderO intHolder;
        [IOCCConstructor]
        public UnmarkedParameter(
          [IOCCBeanReference]IntHolderO intHolder
          ,int abc
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
    public class IntHolderO
    {
        public readonly int heldValue = 42;
    }
}