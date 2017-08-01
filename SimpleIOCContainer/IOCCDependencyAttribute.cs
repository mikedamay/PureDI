using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class IOCCDependencyAttribute : Attribute
    {
        public string Name = "";
        public string Profile = "";
        public IOCC.OS OS = IOCC.OS.Any;
    }
}
