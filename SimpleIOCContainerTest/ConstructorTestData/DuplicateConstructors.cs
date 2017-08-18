using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class DuplicateConstructors : IResultGetter
    {
        private IntHolder intHolder;
        [IOCCConstructor]
        public DuplicateConstructors(
          [IOCCBeanReference]IntHolder intHolder
          )
        {
            this.intHolder = intHolder;
        }
        [IOCCConstructor]
        public DuplicateConstructors(
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