using System;
using PureDI.Common;
using PureDI.Public;

namespace PureDI
{
    /// <summary>
    /// Any class to be injected into another class and
    /// any class injected into should be annotated with an
    /// attribute derived from this base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public abstract class BeanBaseAttribute : Attribute
    {
        /// <remarks>
        /// The name is case insensitive.
        /// Omitting the name from the attribute is equivalent to giving
        /// it a name of "".
        /// <see cref="BeanReferenceAttribute"></see>
        /// </remarks>
        /// <summary>
        /// Give different implementations of the same base class or interface
        /// different names if you intend to reference them through the
        /// interface or base class in member variables
        /// </summary>
        public string  Name
        {
            get { return name; }
            set { name = value.ToLower(); }
        }
        private string name = Constants.DefaultBeanName;
        /// <summary>
        /// beans which have a non-empty profile are only instantiated
        /// when that profile is passed to CreateAndInjectDependencies
        /// </summary>
        /// <example>[Bean(Profile="testonly")]</example>
        /// <conceptualLink target="DI-Profiles">Profiles</conceptualLink>
        public string Profile { get; set; } = Constants.DefaultProfileArg;

        // <example>`[Bean(OS=PDependencyInjector.OS.Linux)]`</example>

        /// <summary>
        /// A class to which this attribute is applied
        /// will be instantiated only for a specific OS.  When running
        /// on any other OS it will be ignored.
        /// OS.Any means that it will always be instantiated unless
        /// there is an alternative with a specific OS.
        /// </summary>
        public Os OS { get; set; } = Os.Any;
    }

    /// <summary>
    /// annotate objects with this class so that they are
    /// candidates for scanning by the injection mechanism.
    /// The injection mechanism will instantiate them if 
    /// a reference <codeInline>[BeanReference]</codeInline> to them is found.
    /// </summary>
    /// <inheritdoc cref="BeanBaseAttribute"/>
    /// <example>[Bean] private SomeOtherBean other;</example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class BeanAttribute : BeanBaseAttribute
    {
    }
}
