using com.TheDisappointedProgrammer.IOCC;
using WithNames = IOCCTest.TestCode.WithNames;

namespace PureDITest.TestCode
{
    [Bean]
    public class BaseBean : ActualBase
    {
    }
    public abstract class ActualBase
    {
        [BeanReference] public SomeRef someRef = null;
    }
    [Bean]
    public class SomeRef
    {
    }
}
