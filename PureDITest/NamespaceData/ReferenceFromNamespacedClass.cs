using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

[Bean]
public class Referred
{
        
}

namespace IOCCTest.NoNamespaceData
{
    [Bean]
    public class ReferenceFromNamespacedClass : IResultGetter
    {
        [BeanReference]
        private Referred referred;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Referred = referred;
            return eo;
        }
    }
}
