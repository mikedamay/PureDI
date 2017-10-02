using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{

    public class MyBeanAttribute : BeanBaseAttribute 
    {
    
    }

    public class MyReferenceAttribute : BeanReferenceBaseAttribute
    {
        
    }

    public class MyConstructorAttribute : ConstructorBaseAttribute
    {
        
    }

    [Bean]
    public class WithNames : IResultGetter
    {
        [MyReference(Name="MyName", ConstructorName="MyConstructorName")] private Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = child.val;     // s/be 42
            return eo;
        }
    }

    [MyBean(Name="MyName")]
    public class Child : ISomeInterface
    {
        public int val;
        [MyConstructor(Name="MyConstructorName")]
        public Child([MyReference(Name="MyName")] GrandChild grandChild)
        {
            this.val = grandChild.val;
        }
    }

    [Bean(Name = "MyName")]
    public class GrandChild
    {
        public int val = 42;
    }

    [MyBean]
    public class Child2 : ISomeInterface
    {
        
    }
    public interface ISomeInterface
    {
    }
}