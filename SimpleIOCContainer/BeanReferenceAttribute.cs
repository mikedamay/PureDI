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
        public string Name
        {
            get => name;
            set => name = value.ToLower();
        }

        public string ConstructorName
        {
            get => constructorName;
            set => constructorName = value.ToLower();
        }
        private string name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        public Type Factory = null;
        public object FactoryParameter = null;
        public BeanScope Scope = BeanScope.Singleton;
        private string constructorName = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
}