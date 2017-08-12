using System.Dynamic;
using System.Linq.Expressions;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    public class SelfReferring : IResultGetter
    {
#pragma warning disable 649
        [IOCCBeanReference] private SelfReferring child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }
    }
    [IOCCBean]
    public class CyclicalDependency : IResultGetter
    {
        [IOCCBeanReference]
        public Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }

    }
    [IOCCBean]
    public class Child : IResultGetter
    {
        [IOCCBeanReference]
        public CyclicalDependency parent;

        [IOCCBeanReference]
        public GrandChild grandChild;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Parent = parent;
            eo.GrandChild = grandChild;
            return eo;
        }
    }
    [IOCCBean]
    public class GrandChild : IResultGetter
    {
        [IOCCBeanReference]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [IOCCBean]
    class ParentWithInterface : IParent, IResultGetter
    {
        [IOCCBeanReference]
        private IChild child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IChild = child;
            return eo;
        }
    }

    internal interface IChild
    {
    }

    internal interface IParent
    {
    }

    [IOCCBean]
    internal class ChildWithInterface : IChild, IResultGetter
    {
        [IOCCBeanReference]
        private IParent parent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IParent = parent;
            return eo;
        }
    }

    internal class BasestClass
    {
        
    }
    [IOCCBean]
    internal class BaseClass : BasestClass
    {
        [IOCCBeanReference] private ChildClass childClass;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            return eo;
        }
    }
    [IOCCBean]
    internal class ChildClass : BaseClass
    {
        #pragma warning disable 414     // field not used
        (int i, string s) iiss = (1, "");
        [IOCCBeanReference] private BasestClass basestClass;
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            return eo;
        }
    }

    [IOCCBean]
    internal class ClassWithMultipleInterfaces : Interface1, Interface2, IResultGetter
    {
        [IOCCBeanReference] private Interface1 interface1;
        [IOCCBeanReference] private Interface1 interface2;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Interface1 = interface1;
            eo.Interface2 = interface2;
            return eo;
        }
    }

    internal interface Interface2
    {
    }

    internal interface Interface1
    {
    }
}