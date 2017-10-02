using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.TestCode.WithNames
{ 
    [Bean(Name="name-A")]
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
    public class CyclicalDependencyAlt : IResultGetter
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
        [BeanReference(Name="name-A")]
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
        [BeanReference(Name="name-A")]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [Bean(Name = "name-B")]
    class ParentWithInterface : IParent, IResultGetter
    {
        
        [BeanReference]
        private IChild child = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.IChild = child;
            eo.Name = "name-B";
            return eo;
        }
    }
    [Bean(Name = "name-B2")]
    class ParentWithInterface2 : IParent, IResultGetter
    {
        [BeanReference]
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

    [Bean]
    internal class ChildWithInterface : IChild, IResultGetter
    {
        [BeanReference(Name = "name-B")]
        private IParent parent = null;
        [BeanReference(Name = "name-B2")]
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
    [Bean(Name="basest")]
    internal class BaseClass : BasestClass, IResultGetter
    {
        [BeanReference(Name="child")] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "basest";
            return eo;
        }
    }
    [Bean]
    internal class BaseClass2 : BasestClass
    {
        [BeanReference] private ChildClass childClass = null;
        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            eo.Name = "";
            return eo;
        }
    }
    [Bean(Name="child")]
    internal class ChildClass : BaseClass
    {
        #pragma warning disable 649
        [BeanReference(Name="basest")] private BasestClass basestClass = null;
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            eo.Name = "child";
            return eo;
        }
    }
}