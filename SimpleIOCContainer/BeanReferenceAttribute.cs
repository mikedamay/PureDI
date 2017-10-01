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
        /// <see>BeanReferenceAttribute.FactoryParameter</see>
        /// </summary>
        /// <example>[Bean(Factory=typeof(MyFactory), FactoryParameter=42)] private int theUltimateQuestion;</example>
        public object FactoryParmeter { get; }
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
    /// Note: a derived class (i.e. any factory) must also be marked as a bean.
    /// </summary>
    /// <see>docs://Simple/UserGuide/Factory</see>
    [IOCCIgnore]
    public interface IFactory
    {
        (object bean, InjectionState injectionState) Execute(
          InjectionState injectionState, BeanFactoryArgs args);
    }
    /// <summary>
    /// Any member which will have some bean assigned to it (injected into it)
    /// must be annotated with an attribute derived from
    /// the bean reference base class.  Typically the <code>[BeanReference]</code>
    /// concrete class will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public abstract class BeanReferenceBaseAttribute : Attribute
    {
        /// <summary>
        /// Where a member type is given as an interface or a  base class
        /// and there is more than one implementation available to the
        /// injection mechansim then the selection depends on matching
        /// the name given in the bean reference to that on a candidate
        /// implementation.
        /// </summary>
        /// <see cref="BeanAttribute.Name"/>
        public string Name
        {
            get => name;
            set => name = value.ToLower();
        }
        /// <summary>
        /// Where a bean has multiple constructors that injection mechanism
        /// might select to instantiate the bean, the constructor name
        /// is used to make the selection by matching it to the
        /// Name on the ConstructorAttribute.
        /// </summary>
        /// <see cref="ConstructorAttribute.Name"/>
        public string ConstructorName
        {
            get => constructorName;
            set => constructorName = value.ToLower();
        }
        private string name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        /// <summary>
        /// optionally a bean reference can delegate injection to a
        /// factory which provides considerable flexibility for the
        /// mechanism.
        /// </summary>
        /// <see cref="IOCC-Factory"/>
        public Type Factory = null;
        /// <summary>
        /// When a bean is injected by a factory the factory parameter
        /// is available to the execute method to provide ancillary
        /// information
        /// </summary>
        public object FactoryParameter = null;
        /// <summary>
        /// There is typically only one bean of each type
        /// instantiated.  Multiple bean references point
        /// to the same object.  This comes about because the
        /// default scope is <code>BeanScope.Singleton</code>.
        /// Where this behaviour is not desired the bean reference
        /// scope can be specified as <code>BeanScope.Prototype</code>.
        /// In this case each bean reference will point to a
        /// separate instance.
        /// </summary>
        /// <see cref="BeanScope"/>
        public BeanScope Scope = BeanScope.Singleton;
        private string constructorName = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
    /// <summary>
    /// Any member variable or constructor parameter annotated with this attribute
    /// will be assigned an appropriate object
    /// </summary>
    /// <example>[BeanReference] private SomecomponentClass component;</example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      | AttributeTargets.Parameter
      , Inherited = false, AllowMultiple = false)]
    public sealed class BeanReferenceAttribute : BeanReferenceBaseAttribute
    {

    }
}