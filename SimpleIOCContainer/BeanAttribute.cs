using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Any class to be injected into another class and
    /// any class injected into should be annotated with an
    /// attribute derived from this base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public abstract class BeanBaseAttribute : Attribute
    {
        /// <remarks>The name is case insensitive</remarks>
        /// <see cref="BeanReferenceAttribute"></see>
        public string  Name
        {
            get { return name; }
            set { name = value.ToLower(); }
        }
        private string name = PDependencyInjector.DEFAULT_BEAN_NAME;
        /// <summary>
        /// beans which have a non-empty profile are only instantiated
        /// when that profile is passed to CreateAndInjectDependencies
        /// </summary>
        /// <example>[Bean(Profile="testonly")]</example>
        /// <conceptualLink target="IOCC-Profiles">Profiles</conceptualLink>
        public string Profile = PDependencyInjector.DEFAULT_PROFILE_ARG;
        /// <summary>
        /// A class to which this attribute is applied can specify
        /// an OS for which it should be instantiated.  When running
        /// on any other OS it will be ignored.
        /// OS.Any means that it will always be instantiated unless
        /// <example>`[Bean(OS=PDependencyInjector.OS.Linux)]`</example>
        /// </summary>
        public PDependencyInjector.OS OS = PDependencyInjector.OS.Any;
    }

    /// <summary>
    /// annotate objects with this class so that they are
    /// candidates for scanning by the injection mechanism.
    /// The injection mechanism will instantiate them if 
    /// a reference <code>[BeanReference]</code> to them is found.
    /// </summary>
    /// <example>[Bean] private SomeOtherBean other;</example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class BeanAttribute : BeanBaseAttribute
    {
    }
}
