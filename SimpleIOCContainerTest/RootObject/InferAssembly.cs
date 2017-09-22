using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.RootObject
{
    [Bean]
    public class InferAssembly
    {
        [BeanReference] public InsertedAsObject inserted = null;
    }
    [Bean]
    public class InsertedAsObject
    {
    }
}