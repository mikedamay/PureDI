using com.TheDisappointedProgrammer.IOCC;

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