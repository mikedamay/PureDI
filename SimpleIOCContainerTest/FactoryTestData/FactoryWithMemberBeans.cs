using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    internal class MemberFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new MemberBean();
        }
    }
    [Bean]
    public class FactoryWithMemberBeans : IResultGetter
    {
        [BeanReference(Factory = typeof(MemberFactory))]
        private MemberBean Member;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Member = Member;
            return eo;
        }
    }
    [Bean]
    public class MemberBean : IResultGetter
    {
        [BeanReference] public SubMember subMember;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SubMember = subMember;
            return eo;
        }
    }
    [Bean]
    public class SubMember
    {

    }
}