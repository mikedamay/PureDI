using System.Dynamic;
using PureDI;

namespace IOCCTest.TestCode
{
    [Bean]
    public class ReadOnlyFields : IResultGetter
    {
        [BeanReference] private readonly ReadOnlyFields field = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
    [Bean]
    public class AlreadyInitialized : IResultGetter
    {
        [BeanReference] private readonly ReadOnlyFields field = new ReadOnlyFields();

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Field = field;
            return eo;
        }
    }
}