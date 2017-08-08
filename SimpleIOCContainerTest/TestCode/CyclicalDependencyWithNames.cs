using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.TestCode.WithNames
{ 
    [IOCCDependency(Name="name-A")]
    public class CyclicalDependency : IResultGetter
    {
        [IOCCInjectedDependency]
        public Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }

    }
    [IOCCDependency]
    public class CyclicalDependencyAlt : IResultGetter
    {
        [IOCCInjectedDependency]
        public Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }

    }
    [IOCCDependency]
    public class Child : IResultGetter
    {
        [IOCCInjectedDependency(Name="name-A")]
        public CyclicalDependency parent;

        [IOCCInjectedDependency]
        public GrandChild grandChild;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Parent = parent;
            eo.GrandChild = grandChild;
            return eo;
        }
    }
    [IOCCDependency]
    public class GrandChild : IResultGetter
    {
        [IOCCInjectedDependency(Name="name-A")]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [IOCCDependency(Name = "name-B")]
    class ParentWithInterface : IParent, IResultGetter
    {
        
        [IOCCInjectedDependency]
        private IChild child = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IChild = child;
            eo.Name = "name-B";
            return eo;
        }
    }
    [IOCCDependency(Name = "name-B2")]
    class ParentWithInterface2 : IParent, IResultGetter
    {
        [IOCCInjectedDependency]
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

    [IOCCDependency]
    internal class ChildWithInterface : IChild, IResultGetter
    {
        [IOCCInjectedDependency(Name = "name-B")]
        private IParent parent = null;
        [IOCCInjectedDependency(Name = "name-B2")]
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
    [IOCCDependency(Name="basest")]
    internal class BaseClass : BasestClass, IResultGetter
    {
        [IOCCInjectedDependency(Name="child")] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "basest";
            return eo;
        }
    }
    [IOCCDependency]
    internal class BaseClass2 : BasestClass
    {
        [IOCCInjectedDependency] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "";
            return eo;
        }
    }
    [IOCCDependency(Name="child")]
    internal class ChildClass : BaseClass
    {
        #pragma warning disable 649
        [IOCCInjectedDependency(Name="basest")] private BasestClass basestClass = null;
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            eo.Name = "child";
            return eo;
        }
    }
}