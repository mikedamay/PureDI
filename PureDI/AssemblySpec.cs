using System;
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
        /// <param name="assemblies">assemblies required to be included in the injection</param>
        public AssemblySpec(params Assembly[] assemblies)
        {
            _explicitAssemblies = assemblies;
            this.IsEmpty = assemblies.Length == 0;
        }

        /// <summary>
        /// empty assembly spec
        /// </summary>
        private AssemblySpec()
        {
            _explicitAssemblies = new Assembly[0];
//            _explicitAssemblies = ImmutableArray.Create(new Assembly[] {this.GetType().Assembly});
            this.IsEmpty = true;
        }

        private static AssemblySpec _empty = new AssemblySpec();
        /// <summary>
        /// AssemblySpec.Empty serves up a reliable emtpy spec.
        /// </summary>
        public static AssemblySpec Empty => _empty;
        /// <summary>
        /// see constructor
        /// </summary>
        public Assembly[] ExplicitAssemblies {
            get { return _explicitAssemblies; } }

        /// <summary>
        /// true indicates that the assembly spec contains no assemblies
        /// </summary>
        public Boolean IsEmpty { get; }
        private Assembly[] _explicitAssemblies;
    }
}