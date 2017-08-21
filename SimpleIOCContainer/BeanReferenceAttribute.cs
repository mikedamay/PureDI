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
    [IOCCIgnore]
    public interface IFactory
    {
        object Execute(BeanFactoryArgs args);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public class BeanReferenceAttribute : Attribute
    {
        public string Name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        public Type Factory = null;
        public object FactoryParameter = null;
        public BeanScope Scope = BeanScope.Singleton;
        public string ConstructorName = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
}