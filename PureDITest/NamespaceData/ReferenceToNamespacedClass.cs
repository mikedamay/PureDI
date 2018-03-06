using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.NoNamespaceData
{
    [Bean]
    public class Referred
    {
        
    }
}

[Bean]
public class ReferenceToNamespacedClass : IResultGetter
{
    [BeanReference]
    private IOCCTest.NoNamespaceData.Referred referred;

    public dynamic GetResults()
    {
        dynamic eo = new ExpandoObject();
        eo.Referred = referred;
        return eo;
    }
}
