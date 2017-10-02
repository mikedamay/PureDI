using System.Dynamic;
using System.Reflection.Emit;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ConstructorTestData
{

    [Bean]
    public class NamedRootConstructor
        : IResultGetter
    {
        private ActualDerivedClass actual;

        public NamedRootConstructor()
        {
            
        }
        [Constructor(Name="TestConstructor")]
        public NamedRootConstructor([BeanReference]ActualDerivedClass actual)
        {
            this.actual = actual;
        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Stuff = actual?.stuff;
            return eo;
        }
    }

    [Bean]
    public class ActualDerivedClass
    {
        public string stuff = "stuff";
    }
}