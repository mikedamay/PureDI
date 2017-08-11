using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    internal class Generic<T>
    {
        public string Name => "Generic<T>";
    }
    [IOCCDependency]
    internal class RefersToGeneric
    {
        [IOCCInjectedDependency] private Generic<int> genericInt;
        public Generic<int> GenericInt => genericInt;
    }

    [IOCCDependency]
    internal class GenericHolderParent
    {
        [IOCCInjectedDependency]
        private GenericHolder<GenericHeld> genericHolder;
        public GenericHolder<GenericHeld> GenericHolder => genericHolder;
    }
    [IOCCDependency]
    internal class GenericHolder<T>
    {
        [IOCCInjectedDependency] private T injectedT;
        public T GenericHeld => injectedT;
    }

    [IOCCDependency]
    internal class GenericHeld
    {
        public string Name => "GenericHeld";
    }
}