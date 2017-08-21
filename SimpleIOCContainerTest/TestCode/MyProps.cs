using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [Bean]
    public class MyProps : IResultGetter
    {
        private MyProps myProps;
        [BeanReference]
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
    [Bean]
    public class MyAutoProp : IResultGetter
    {
        [BeanReference(Name= "name-SomeOtherProp2")]
        private ISomeOtherProp MyProp { get; }

        public virtual dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyProp = MyProp;
            return eo;
        }
    }

    [Bean(Name="name-SomeOtherProp")]
    public class SomeOtherProp : ISomeOtherProp
    {
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Name = "name-SomeOtherProp";
            return eo;
        }
        
    }
    [Bean(Name="name-SomeOtherProp2")]
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