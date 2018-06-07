using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    [Bean]
    public class UnnamedBean
    {
        [BeanReference] public ChildOfUnnamed child;
    }

    [Bean]
    public class ChildOfUnnamed
    {
    }

    [Bean]
    public class Bean
    {
        [BeanReference(Name="BeanWithAName")] public UnnamedBean RefToUnnamedBean;
    }
}