using System;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Contains all the functionality of [Constructor]
    /// attribute.
    /// 
    /// There is no use case for this base class.  It
    /// is present for consistency.  I suppose if a
    /// library user wanted to combine the [Constructor]
    /// attribute with their own bean they could use the
    /// base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public abstract class ConstructorBaseAttribute : Attribute
    {
        public string Name = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ConstructorAttribute : ConstructorBaseAttribute
    {
    }
}