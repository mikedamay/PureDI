using System.Runtime.InteropServices;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class Unreachable : IResultGetter
    {
        public dynamic GetResults()
        {
            return null;
        }
    }
    public static class ActuallyUnreachable
    {
        [BeanReference] private static NonStaticClass nonStatic;
    }
    [Bean]
    internal class NonStaticClass
    {
    }
}