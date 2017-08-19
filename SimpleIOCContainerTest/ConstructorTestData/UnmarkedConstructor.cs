using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class UnmarkedConstructor : IResultGetter
    {
        private IntHolder intHolder;
        public UnmarkedConstructor(
            [IOCCBeanReference]IntHolder intHolder
            , int abc
        )
        { }
        [IOCCConstructor]
        public UnmarkedConstructor(
            [IOCCBeanReference]IntHolder intHolder
            , [IOCCBeanReference] IntHolder intHolder2
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
    public class IntHolder
    {
        public readonly int heldValue = 42;
    }
}