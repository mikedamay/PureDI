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

    [IOCCDependency]
    internal class GenericWith3Params<A, B, C>
    {
        
    }
    [IOCCDependency]
    internal class MultipleParamGenericUser
    {
        public GenericWith3Params<int, int, int> Multiple => multiple;
        [IOCCInjectedDependency]
        private GenericWith3Params<int, int, int> multiple;
    }

    [IOCCDependency]
    internal class NestedGeneric<T>
    {
        
    }
    [IOCCDependency]
    internal class WrapperGeneric<T>
    {
        
    }
    [IOCCDependency]
    internal class WrapperUser
    {
        public WrapperGeneric<NestedGeneric<int>> Nested => nested;
        [IOCCInjectedDependency]
        private WrapperGeneric<NestedGeneric<int>> nested;
    }
}