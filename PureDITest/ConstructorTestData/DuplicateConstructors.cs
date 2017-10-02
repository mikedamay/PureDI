using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class DuplicateConstructors : IResultGetter
    {
        private IntHolder intHolder;
        [Constructor]
        public DuplicateConstructors(
          [BeanReference]IntHolder intHolder
          )
        {
            this.intHolder = intHolder;
        }
        [Constructor]
        public DuplicateConstructors(
            [BeanReference]IntHolder intHolder
            , [BeanReference] IntHolder intHolder2
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
    public class IntHolder
    {
        public readonly int heldValue = 42;
    }
}