using PureDI.Public;
using System.Collections.Immutable;
using System.Reflection;

namespace PureDI
{
    /// <summary>
    /// An object of this class lists assemblies that should be included in the injection process
    /// </summary>
    public class AssemblySpec
    {
        /// <summary>
        /// creates readonly object
        /// </summary>
        /// <param name="exclude">2 assemblies are included by default unless this parameter is specified</param>
        /// <param name="assemblies">assemblies required to be included in the injection</param>
        public AssemblySpec(AssemblyExclusion exclude = AssemblyExclusion.ExcludedNone, params Assembly[] assemblies)
        {
            ExplicitAssemblies = ImmutableArray.Create(assemblies);
            ExcludedAssemblies = exclude;
        }
        /// <summary>
        /// see constructor
        /// </summary>
        private AssemblyExclusion ExcludedAssemblies { get; }
        /// <summary>
        /// see constructor
        /// </summary>
        private ImmutableArray<Assembly> ExplicitAssemblies { get; }        
    }
}