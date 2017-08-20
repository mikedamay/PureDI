using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [IOCCBean]
    public class SimpleScope : IResultGetter
    {
        [IOCCBeanReference(Scope = BeanScope.Prototype)] private MemberClassX MemberA;
        [IOCCBeanReference(Scope = BeanScope.Prototype)] private MemberClassX MemberB;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MemberA = MemberA;
            eo.MemberB = MemberB;
            return eo;
        }
    }
    [IOCCBean]
    internal class MemberClassX
    {
    }
}