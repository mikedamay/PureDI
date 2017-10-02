using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class UnmarkedConstructor : IResultGetter
    {
        private IntHolderY intHolder;
        public UnmarkedConstructor(
            [BeanReference]IntHolderY intHolder
            , int abc
        )
        { }
        [Constructor]
        public UnmarkedConstructor(
            [BeanReference]IntHolderY intHolder
            , [BeanReference] IntHolderY intHolder2
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
    public class IntHolderY
    {
        public readonly int heldValue = 42;
    }
}