using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// an instance of BeanFactoryArgs is passed by the container
    /// to each factory's `Execute` method.
    /// </summary>
    public class BeanFactoryArgs
    {
        /// <summary>
        /// as specified in Bean.FactoryParameter,
        ///  `[Bean(Factory=typeof(MyFactory), FactoryParameter=42)]`
        /// </summary>
        public object FactoryParmeter { get; }
        /// <inheritdoc cref="BeanFactoryArgs"/>>
        /// <param name="FactoryParameter"><see>BeanFactoryArgs.FactoryParameter</see></param>
        public BeanFactoryArgs(object FactoryParameter)
        {
             this.FactoryParmeter = FactoryParameter;
        }
    }
    /// <summary>
    /// Any bean implementing the IFactory interface will
    /// be treated as a factory and have their `Execute` method
    /// invoked when encountered in a bean reference.
    /// Note: a derived class must also be marked as a bean.
    /// </summary>
    /// <see>docs://Simple/UserGuide/Factory</see>
    [IOCCIgnore]
    public interface IFactory
    {
        (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public abstract class BeanReferenceBaseAttribute : Attribute
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public sealed class BeanReferenceAttribute : BeanReferenceBaseAttribute
    {

    }
}