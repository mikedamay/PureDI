using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [Bean]
    public class PrototypesWithSingletons : IResultGetter
    {
        [BeanReference(Scope = BeanScope.Prototype)] private MemberClass MemberA;
        [BeanReference(Scope = BeanScope.Prototype)] private MemberClass MemberB;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MemberA = MemberA;
            eo.MemberB = MemberB;
            return eo;
        }
    }
    [Bean]
    internal class MemberClass : IResultGetter
    {
        [BeanReference] private MemberMemberClass MemberA;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MemberA = MemberA;
            return eo;
        }
    }
    [Bean]
    internal class MemberMemberClass
    {
    }
}