using PureDI;
using PureDI.Attributes;

namespace PureDITest.TestData
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