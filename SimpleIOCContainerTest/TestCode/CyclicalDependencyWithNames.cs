using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.TestCode.WithNames
{ 
    [IOCCBean(Name="name-A")]
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
    public class CyclicalDependencyAlt : IResultGetter
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
        [IOCCBeanReference(Name="name-A")]
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
        [IOCCBeanReference(Name="name-A")]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [IOCCBean(Name = "name-B")]
    class ParentWithInterface : IParent, IResultGetter
    {
        
        [IOCCBeanReference]
        private IChild child = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IChild = child;
            eo.Name = "name-B";
            return eo;
        }
    }
    [IOCCBean(Name = "name-B2")]
    class ParentWithInterface2 : IParent, IResultGetter
    {
        [IOCCBeanReference]
        private IChild child = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IChild = child;
            eo.Name = "name-B2";
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
        [IOCCBeanReference(Name = "name-B")]
        private IParent parent = null;
        [IOCCBeanReference(Name = "name-B2")]
        private IParent parent2 = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IParent = parent;
            eo.IParent2 = parent2;
            eo.Name = "";
            return eo;
        }
    }

    internal class BasestClass
    {
        
    }
    [IOCCBean(Name="basest")]
    internal class BaseClass : BasestClass, IResultGetter
    {
        [IOCCBeanReference(Name="child")] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "basest";
            return eo;
        }
    }
    [IOCCBean]
    internal class BaseClass2 : BasestClass
    {
        [IOCCBeanReference] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "";
            return eo;
        }
    }
    [IOCCBean(Name="child")]
    internal class ChildClass : BaseClass
    {
        #pragma warning disable 649
        [IOCCBeanReference(Name="basest")] private BasestClass basestClass = null;
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            eo.Name = "child";
            return eo;
        }
    }
}