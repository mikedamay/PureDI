using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class UnmarkedMatchingConstructor : IResultGetter
    {
        private IntHolderN intHolder;
        public UnmarkedMatchingConstructor(
            [BeanReference]IntHolderN intHolder
        )
        { }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = intHolder.heldValue;
            return eo;
        }
    }

    [Bean]
    public class IntHolderN
    {
        public readonly int heldValue = 42;
    }
}