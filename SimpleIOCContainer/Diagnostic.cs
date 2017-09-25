using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Typical usage:
    ///     Diagnostic diag = diagnostics.groups["InvalidBeanType"].CreateDiagnostic();
    ///     diag.bean = someType.GetIOCCName();
    ///     diag.dependentBean = someOtherType.GetIOCCName();
    ///     diagnostics.groups["InvalidBeanType"].Add(diag);
    /// alternatively:
    ///     Diagnostic diag = diagnostics.groups["SomeOthertopic"].CreateDiagnostic();
    ///     diag.someField = someValue;
    ///     diag.someField2 = someValue2
    ///     diagnostics.Groups["SomeOthertopic"].Add(diag);
    /// The cause codes and the members of diag must tie up with DiagnosticSchema.xml
    /// </summary>
    public abstract class Diagnostic : DynamicObject
    {
        private readonly IOCCDiagnostics.Group group;
        internal IDictionary<string, object> Members { get; }

        protected Diagnostic(IOCCDiagnostics.Group group)
        {
            this.group = group;
            Members = CreateArtefactMap(group.ArtefactSchema);
        }

        private IDictionary<string, object> 
          CreateArtefactMap(ISet<string> groupArtefactSchema)
        {
            return groupArtefactSchema.ToDictionary<
              string, string, object>(a => a, a => null);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!Members.ContainsKey(binder.Name))
            {
                result = null;
                return false;
            }
            result = Members[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!Members.ContainsKey(binder.Name))
            {
                return false;
            }
            Members[binder.Name] = value;
            return true;
        }
    }

}