using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Typically this is applied to interface or possibly base class
    /// which the library user does not want to be treated as a reference
    /// for the purposes of injecting dependencies.
    /// This does not prevent it being used as the root type in
    /// calls to IOCC.GetOrCreateObjectTree().
    /// The purpose is simply to prevent warnings for duplicate
    /// beans which might otherwise occurs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class
      ,Inherited = false, AllowMultiple = false)]
    public class IOCCIgnoreAttribute : Attribute
    {
        
    }
}