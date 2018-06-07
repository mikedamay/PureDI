using System;

namespace PureDI.Attributes
{
    /// <summary>
    /// Contains all the functionality of [IOCCIgnore]
    /// attribute.
    /// 
    /// There is no use case for this base class.  It
    /// is present for consistency.  I suppose if a
    /// library user wanted to combine the [IOCCIgnore]
    /// attribute with their own bean they could use the
    /// base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class
        , Inherited = false, AllowMultiple = false)]
    public abstract class IgnoreBaseAttribute : Attribute
    {

    }

    /// <summary>
    /// Typically this is applied to interface or possibly base class
    /// which the library user does not want to be treated as a reference
    /// for the purposes of injecting dependencies.
    /// This does not prevent it being used as the root type in
    /// calls to DependencyInjector.CreateAndInjectDependencies().
    /// The purpose is simply to prevent warnings for duplicate
    /// beans which might otherwise occurs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class
        , Inherited = false, AllowMultiple = false)]
    public sealed class IgnoreAttribute : IgnoreBaseAttribute
    {

    }
}