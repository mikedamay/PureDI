using System;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public abstract class ConstructorBaseAttribute : Attribute
    {
        public string Name = SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
    }
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class ConstructorAttribute : ConstructorBaseAttribute
    {
    }
}