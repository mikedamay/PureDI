using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode.WithNames
{
    [Bean(Name = "some-name")]
    public class InheritedBean
    {
        [BeanReference] public DerivedBean derived;
    }

    [Bean]
    public class DerivedBean : InheritedBean
    {

    }

    [Bean]
    public class BeanUser
    {
        [BeanReference] public InheritedBeanWithProfile Used;
    }

    [Bean]
    public class InheritedBeanWithProfile
    {
        public virtual string Val => "Inherited";
    }

    [Bean(Profile = "some-profile")]
    public class DerivedBeanWithProfile : InheritedBeanWithProfile
    {
        public override string Val => "Derived";

    }
    [Bean]
    public class IgnoredBeanUser
    {
        [BeanReference] public InheritedIgnorer InheritedIgnorer;
        [BeanReference] public DerivedFromIgnorer DerivedFromIgnorer;
    }
    [IOCCIgnore]
    [Bean]
    public class InheritedIgnorer
    {
        public virtual string Val => "Inherited and ignored";
    }

    [Bean]
    public class DerivedFromIgnorer : InheritedIgnorer
    {
        public override string Val => "Derived from ignorer";

    }
}