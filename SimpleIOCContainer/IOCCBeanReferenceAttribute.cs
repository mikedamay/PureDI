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
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public class IOCCBeanReferenceAttribute : Attribute
    {
        public string Name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        public Type Factory = null;
        public object FactoryParameter = null;
        public BeanScope Scope = BeanScope.Singleton;
        public string ConstructorName = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
}