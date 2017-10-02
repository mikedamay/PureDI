using System.Dynamic;
using System.Linq.Expressions;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [Bean]
    public class SelfReferring : IResultGetter
    {
#pragma warning disable 649
        [BeanReference] private SelfReferring child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }
    }
    [Bean]
    public class CyclicalDependency : IResultGetter
    {
        [BeanReference]
        public Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }

    }
    [Bean]
    public class Child : IResultGetter
    {
        [BeanReference]
        public CyclicalDependency parent;

        [BeanReference]
        public GrandChild grandChild;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Parent = parent;
            eo.GrandChild = grandChild;
            return eo;
        }
    }
    [Bean]
    public class GrandChild : IResultGetter
    {
        [BeanReference]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [Bean]
    class ParentWithInterface : IParent, IResultGetter
    {
        [BeanReference]
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

    [Bean]
    internal class ChildWithInterface : IChild, IResultGetter
    {
        [BeanReference]
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
    [Bean]
    internal class BaseClass : BasestClass
    {
        [BeanReference] private ChildClass childClass;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            return eo;
        }
    }
    [Bean]
    internal class ChildClass : BaseClass
    {
        #pragma warning disable 414     // field not used
        (int i, string s) iiss = (1, "");
        [BeanReference] private BasestClass basestClass;
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            return eo;
        }
    }

    [Bean]
    internal class ClassWithMultipleInterfaces : Interface1, Interface2, IResultGetter
    {
        [BeanReference] private Interface1 interface1;
        [BeanReference] private Interface1 interface2;

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