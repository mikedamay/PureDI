using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class BeanFactoryArgs
    {
        public object FactoryParmeter { get; }

        public BeanFactoryArgs(object FactoryParameter)
        {
             this.FactoryParmeter = FactoryParameter;
        }
    }
    public interface IOCCFactory
    {
        object Execute(BeanFactoryArgs args);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      , Inherited = false, AllowMultiple = false)]
    public class IOCCBeanReferenceAttribute : Attribute
    {
        public string Name = "";
        public Type Factory = null;
        public object FactoryParameter = null;
    }
}