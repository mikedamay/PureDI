using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{
    [IOCCBean]
    public class SimpleConstructor : IResultGetter
    {
        private int someValue;
        [IOCCConstructor]
        public SimpleConstructor(
          [IOCCBeanReference]int someValue
          )
        {
            this.someValue = someValue;
        }

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeValue = someValue;
            return eo;
        }
    }
}