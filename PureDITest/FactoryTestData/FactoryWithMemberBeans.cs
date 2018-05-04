using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Bean]
    internal class MemberFactory : IFactory
    {
        [BeanReference] private PDependencyInjector injector = null;
        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<MemberBean>(injectionState);
        }
    }
    [Bean]
    public class FactoryWithMemberBeans : IResultGetter
    {
        [BeanReference(Factory = typeof(MemberFactory))]
        private MemberBean Member = null;

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