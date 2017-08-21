using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class UnmarkedParameter : IResultGetter
    {
        private IntHolderO intHolder;
        [Constructor]
        public UnmarkedParameter(
          [BeanReference]IntHolderO intHolder
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

    [Bean]
    public class IntHolderO
    {
        public readonly int heldValue = 42;
    }
}