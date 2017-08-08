using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    public class MyProps : IResultGetter
    {
        private MyProps myProps;
        [IOCCInjectedDependency]
        private MyProps MyProp { get { return myProps; } set { myProps = value; }}

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyProp = MyProp;
            return eo;
        }
    }

    //[IOCCDependency(Name = "name-MyProps2")]
    public class MyProps2 : MyAutoProp
    {
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-MyProp2";
            return eo;
        }
        
    }
    [IOCCDependency]
    public class MyAutoProp : IResultGetter
    {
        [IOCCInjectedDependency(Name= "name-SomeOtherProp2")]
        private ISomeOtherProp MyProp { get; }

        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyProp = MyProp;
            return eo;
        }
    }

    [IOCCDependency(Name="name-SomeOtherProp")]
    public class SomeOtherProp : ISomeOtherProp
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-SomeOtherProp";
            return eo;
        }
        
    }
    [IOCCDependency(Name="name-SomeOtherProp2")]
    public class SomeOtherProp2 : ISomeOtherProp
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-SomeOtherProp2";
            return eo;
        }
       
    }

    internal interface ISomeOtherProp
    {
        
    }
}