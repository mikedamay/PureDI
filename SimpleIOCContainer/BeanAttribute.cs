using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// Any class to be injected into another class and
    /// any class injected into.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public abstract class BeanBaseAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string  Name
        {
            get { return name; }
            set { name = value.ToLower(); }
        }
        private string name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        /// <summary>
        /// beans which have a non-empty profile are only instantiated
        /// when that profile is passed to CreateAndInjectDependencies
        /// </summary>
        /// <example>[Bean(Profile="testonly")]</example>
        /// <see>/Simple/UserGuide/Profiles</see>
        public string Profile = SimpleIOCContainer.DEFAULT_PROFILE_ARG;
        /// <summary>
        /// A class to which this attribute is applied can specify
        /// an OS for which it should be instantiated.  When running
        /// on any other OS it will be ignored.
        /// OS.Any means that it will always be instantiated unless
        /// <example>`[Bean(OS=SimpleIOCContainer.OS.Linux)]`</example>
        /// </summary>
        public SimpleIOCContainer.OS OS = SimpleIOCContainer.OS.Any;
    }

    /// <inheritdoc/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class BeanAttribute : BeanBaseAttribute
    {
    }
}
