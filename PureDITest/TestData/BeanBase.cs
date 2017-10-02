using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCContainerTest.TestData
{

    public class IBean
    {

    }

    [Bean]
    public class BeanBase : IBean
    {

    }

    [Bean]
    public class ABean : BeanBase
    {

    }
}