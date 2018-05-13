using PureDI.Attributes;

namespace IOCCTest.rootBean
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