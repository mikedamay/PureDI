using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public abstract class BeanBaseAttribute : Attribute
    {
        public string  Name
        {
            get { return name; }
            set { name = value.ToLower(); }
        }
        private string name = SimpleIOCContainer.DEFAULT_BEAN_NAME;
        public string Profile = SimpleIOCContainer.DEFAULT_PROFILE_ARG;
        public SimpleIOCContainer.OS OS = SimpleIOCContainer.OS.Any;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class BeanAttribute : BeanBaseAttribute
    {
    }
}
