using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PureDI
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
        private readonly Diagnostics.Group group;
        internal IDictionary<string, object> Members { get; }

        internal Diagnostic(Diagnostics.Group group)
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
        /// <summary>
        /// strictly here to fulfill our obligations as a dynamic object
        /// </summary>
        /// <param name="binder">here to fulfill our obligations as a dynamic object</param>
        /// <param name="result">here to fulfill our obligations as a dynamic object</param>
        /// <returns>here to fulfill our obligations as a dynamic object</returns>
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
        /// <summary>
        /// here to fulfill our obligations as a dynamic object
        /// </summary>
        /// <param name="binder">here to fulfill our obligations as a dynamic object</param>
        /// <param name="value">here to fulfill our obligations as a dynamic object</param>
        /// <returns>here to fulfill our obligations as a dynamic object</returns>
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