using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    internal class MemberFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new MemberBean();
        }
    }
    [IOCCBean]
    public class FactoryWithMemberBeans : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(MemberFactory))]
        private MemberBean Member;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Member = Member;
            return eo;
        }
    }
    [IOCCBean]
    public class MemberBean : IResultGetter
    {
        [IOCCBeanReference] public SubMember subMember;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SubMember = subMember;
            return eo;
        }
    }
    [IOCCBean]
    public class SubMember
    {

    }
}