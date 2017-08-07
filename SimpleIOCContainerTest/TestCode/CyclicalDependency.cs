using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
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
    public class Child : IResultGetter
    {
        [IOCCInjectedDependency]
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
        [IOCCInjectedDependency]
        public CyclicalDependency grandParent;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.GrandParent = grandParent;
            return eo;
        }
    }

    [IOCCDependency]
    class ParentWithInterface : IParent, IResultGetter
    {
        [IOCCInjectedDependency]
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

    [IOCCDependency]
    internal class ChildWithInterface : IChild, IResultGetter
    {
        [IOCCInjectedDependency]
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
    [IOCCDependency]
    internal class BaseClass : BasestClass
    {
        [IOCCInjectedDependency] private ChildClass childClass;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildClass = childClass;
            return eo;
        }
    }
    [IOCCDependency]
    internal class ChildClass : BaseClass
    {
        [IOCCInjectedDependency] private BasestClass basestClass;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.BasestClass = basestClass;
            return eo;
        }
    }
}