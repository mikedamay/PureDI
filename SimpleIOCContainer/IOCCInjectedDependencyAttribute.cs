using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class IOCCInjectedDependencyAttribute : Attribute
    {
        public string Name = "";
    }
}