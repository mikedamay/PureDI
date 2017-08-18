using System;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class IOCCConstructorAttribute : Attribute
    {
        public string Name = IOCC.DEFAULT_CONSTRUCTOR_NAME;
    }
}