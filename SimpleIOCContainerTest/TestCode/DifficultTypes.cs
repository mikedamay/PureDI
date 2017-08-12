using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    internal class Generic<T>
    {
        public string Name => "Generic<T>";
    }
    [IOCCBean]
    internal class RefersToGeneric
    {
        [IOCCBeanReference] private Generic<int> genericInt;
        public Generic<int> GenericInt => genericInt;
    }

    [IOCCBean]
    internal class GenericHolderParent
    {
        [IOCCBeanReference]
        private GenericHolder<GenericHeld> genericHolder;
        public GenericHolder<GenericHeld> GenericHolder => genericHolder;
    }
    [IOCCBean]
    internal class GenericHolder<T>
    {
        [IOCCBeanReference] private T injectedT;
        public T GenericHeld => injectedT;
    }

    [IOCCBean]
    internal class GenericHeld
    {
        public string Name => "GenericHeld";
    }

    [IOCCBean]
    internal class GenericWith3Params<A, B, C>
    {
        
    }
    [IOCCBean]
    internal class MultipleParamGenericUser
    {
        public GenericWith3Params<int, int, int> Multiple => multiple;
        [IOCCBeanReference]
        private GenericWith3Params<int, int, int> multiple;
    }

    [IOCCBean]
    internal class NestedGeneric<T>
    {
        
    }
    [IOCCBean]
    internal class WrapperGeneric<T>
    {
        
    }
    [IOCCBean]
    internal class WrapperUser
    {
        public WrapperGeneric<NestedGeneric<int>> Nested => nested;
        [IOCCBeanReference]
        private WrapperGeneric<NestedGeneric<int>> nested;
    }
}