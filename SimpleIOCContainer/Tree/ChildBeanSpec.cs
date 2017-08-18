using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
    internal class ChildBeanSpec
    {
        public MemberInfo FieldOrPropertyInfo
        {
            get { return Info.FieldOrPropertyInfo; }
        }

        public ParameterInfo ParameterInfo
        {
            get { return Info.ParameterInfo; }
        }
        public object MemberOrFactoryBean { get; }
        public bool IsFactory { get; }
        private Info Info { get; }

        public ChildBeanSpec(Info fieldOrPropertyInfo, object memberOrFactoryBean, bool isFactory)
        {
            this.Info = fieldOrPropertyInfo;
            this.MemberOrFactoryBean = memberOrFactoryBean;
            this.IsFactory = isFactory;
        }
    }
}