using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      , Inherited = false, AllowMultiple = false)]
    public class IOCCBeanReferenceAttribute : Attribute
    {
        public string Name = "";
    }
}