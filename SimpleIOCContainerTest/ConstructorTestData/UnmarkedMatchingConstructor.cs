using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class UnmarkedMatchingConstructor : IResultGetter
    {
        private IntHolderN intHolder;
        public UnmarkedMatchingConstructor(
            [IOCCBeanReference]IntHolderN intHolder
        )
        { }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = intHolder.heldValue;
            return eo;
        }
    }

    [IOCCBean]
    public class IntHolderN
    {
        public readonly int heldValue = 42;
    }
}