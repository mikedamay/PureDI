using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCContainerTest.TestData
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