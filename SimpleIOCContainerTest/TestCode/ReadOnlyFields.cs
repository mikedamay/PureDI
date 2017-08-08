using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    public class ReadOnlyFields : IResultGetter
    {
        [IOCCInjectedDependency] private readonly ReadOnlyFields field = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
    [IOCCDependency]
    public class AlreadyInitialized : IResultGetter
    {
        [IOCCInjectedDependency] private readonly ReadOnlyFields field = new ReadOnlyFields();

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
}