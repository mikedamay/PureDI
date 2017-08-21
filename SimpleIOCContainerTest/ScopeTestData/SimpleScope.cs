using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [Bean]
    public class SimpleScope : IResultGetter
    {
        [BeanReference(Scope = BeanScope.Prototype)] private MemberClassX MemberA;
        [BeanReference(Scope = BeanScope.Prototype)] private MemberClassX MemberB;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MemberA = MemberA;
            eo.MemberB = MemberB;
            return eo;
        }
    }
    [Bean]
    internal class MemberClassX
    {
    }
}