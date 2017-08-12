using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    public class ReadOnlyFields : IResultGetter
    {
        [IOCCBeanReference] private readonly ReadOnlyFields field = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
    [IOCCBean]
    public class AlreadyInitialized : IResultGetter
    {
        [IOCCBeanReference] private readonly ReadOnlyFields field = new ReadOnlyFields();

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
}