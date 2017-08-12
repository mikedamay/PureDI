using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    public class MyProps : IResultGetter
    {
        private MyProps myProps;
        [IOCCBeanReference]
        private MyProps MyProp { get { return myProps; } set { myProps = value; }}

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyProp = MyProp;
            return eo;
        }
    }

    public class MyProps2 : MyAutoProp
    {
        public override dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-MyProp2";
            return eo;
        }
        
    }
    [IOCCBean]
    public class MyAutoProp : IResultGetter
    {
        [IOCCBeanReference(Name= "name-SomeOtherProp2")]
        private ISomeOtherProp MyProp { get; }

        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyProp = MyProp;
            return eo;
        }
    }

    [IOCCBean(Name="name-SomeOtherProp")]
    public class SomeOtherProp : ISomeOtherProp
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-SomeOtherProp";
            return eo;
        }
        
    }
    [IOCCBean(Name="name-SomeOtherProp2")]
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