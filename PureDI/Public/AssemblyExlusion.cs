using System;

namespace PureDI.Public
{
    /// <summary>
    /// A parameter of this type can be passed to the constructor
    /// to indicate whether the default scanning of libraries is performed.
    /// </summary>
    /// <conceptualLink target="DI-Assemblies">See the Notes section of Assemblies</conceptualLink>
    [Flags]
    public enum AssemblyExclusion
    {
        /// <summary>
        /// default - the assembly containing the root type will
        /// be scanned for beans as will the PDependencyInjector library
        /// itself
        /// </summary>
        ExcludedNone = 0,
        /// <summary>
        /// The library itself should not be scanned for dependencies
        /// </summary>
        ExcludePDependencyInjector = 1,
        /// <summary>
        /// The assembly containing the type passed to CreateAndInjectDependencies
        /// will not be scanned for beans
        /// </summary>
        ExcludeRootTypeAssembly = 2
    }
}