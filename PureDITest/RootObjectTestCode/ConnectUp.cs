using PureDI;
using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    public class ConnectUp
    {
        [BeanReference] public ExistingChild connectedChild;
    }

    [Bean]
    public class ExistingRoot
    {
        [BeanReference(Scope=BeanScope.Singleton)] public ExistingChild existingChild;
    }
    [Bean]
    public class ExistingChild
    {
    }
}