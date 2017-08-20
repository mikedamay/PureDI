using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class UnmarkedConstructor : IResultGetter
    {
        private IntHolderY intHolder;
        public UnmarkedConstructor(
            [IOCCBeanReference]IntHolderY intHolder
            , int abc
        )
        { }
        [IOCCConstructor]
        public UnmarkedConstructor(
            [IOCCBeanReference]IntHolderY intHolder
            , [IOCCBeanReference] IntHolderY intHolder2
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
    public class IntHolderY
    {
        public readonly int heldValue = 42;
    }
}