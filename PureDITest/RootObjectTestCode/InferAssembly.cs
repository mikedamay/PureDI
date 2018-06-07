using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    [Bean]
    public class InferAssembly
    {
        [BeanReference] public InsertedAsObject inserted = null;
    }
    [Bean]
    public class InsertedAsObject
    {
    }
}