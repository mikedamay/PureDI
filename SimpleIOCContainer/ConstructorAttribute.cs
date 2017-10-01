using System;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Contains all the functionality of [Constructor]
    /// attribute.
    /// 
    /// There is no use case for having a separate base class.  It
    /// is present for consistency.  I suppose if a
    /// library user wanted to combine the [Constructor]
    /// attribute with their own bean they could use the
    /// base class.
    /// </summary>
    /// <remarks>All parameters must be annotated with a <code>[BeanReference]</code> attribute</remarks>
    [AttributeUsage(AttributeTargets.Constructor)]
    public abstract class ConstructorBaseAttribute : Attribute
    {
        /// <summary>
        /// Where there is a choice of constructors for injection
        /// the name acts as a tie breaker.
        /// </summary>
        /// <see cref="BeanReferenceAttribute"/>
        public string Name = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
    /// <summary>
    /// Where the library user needs to inject dependencies through 
    /// a constructor the constructor in question must be annotated
    /// with the <code>[Constructor]</code> attribute.
    /// </summary>
    /// <remarks>All parameters must be annotated with a <code>[BeanReference]</code> attribute.
    /// Note that where injection is made to member variables a no-args constructor must
    /// be present but there is no need to annotate it.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ConstructorAttribute : ConstructorBaseAttribute
    {
    }
}