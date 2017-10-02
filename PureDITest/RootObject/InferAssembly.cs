using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.rootBean
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