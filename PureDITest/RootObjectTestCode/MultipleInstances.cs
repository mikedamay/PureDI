using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    [Bean]
    public class MultipleInstances
    {
        [BeanReference] public Instance ClassInstance = null;
        [BeanReference(Name="MyInstance")] public Instance InstanceInstance = null;

    }

    [Bean]
    public class Instance
    {
    }
}