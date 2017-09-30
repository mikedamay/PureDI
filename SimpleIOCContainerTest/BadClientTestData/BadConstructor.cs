using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.BadClientTestData
{
    [Bean]
    public class BadConstructor
    {
        [BeanReference] private ActualBadConstructor actual = null;

        BadConstructor()
        {
            var x = actual;
        }
    }
    [Bean]
    internal class ActualBadConstructor
    {
        ActualBadConstructor()
        {
            int x  = ((string)null).Length;
        }
    }
}