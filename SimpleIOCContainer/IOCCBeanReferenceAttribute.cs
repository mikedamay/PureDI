using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class BeanFactoryArgs
    {
        public IOCC IOCC { get; }
        public BeanFactoryArgs(IOCC iocc)
        {
            this.IOCC = iocc;
        }
    }

    public delegate void FactoryMethod(BeanFactoryArgs args);

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
      , Inherited = false, AllowMultiple = false)]
    public class IOCCBeanReferenceAttribute : Attribute
    {
        public string Name = "";
        public Type Factory = null;
        public FactoryMethod FactoryMethod = null;
        public string FactoryParameter = "";
    }
}