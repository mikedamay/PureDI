using PureDI;
using PureDI.Attributes;

namespace PureDITest.TestData
{
    public interface INamedBean
    {
        
    }
    [Bean]
    public class NamedBeanBase : INamedBean
    {
        
    }

    [Bean(Name="Derived")]
    public class ANamedBean : NamedBeanBase
    {
        
    }
}