using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [Bean]
    internal class Generic<T>
    {
        public string Name => "Generic<T>";
    }
    [Bean]
    internal class RefersToGeneric
    {
        [BeanReference] private Generic<int> genericInt;
        public Generic<int> GenericInt => genericInt;
    }

    [Bean]
    internal class GenericHolderParent
    {
        [BeanReference]
        private GenericHolder<GenericHeld> genericHolder;
        public GenericHolder<GenericHeld> GenericHolder => genericHolder;
    }
    [Bean]
    internal class GenericHolder<T>
    {
        [BeanReference] private T injectedT;
        public T GenericHeld => injectedT;
    }

    [Bean]
    internal class GenericHeld
    {
        public string Name => "GenericHeld";
    }

    [Bean]
    internal class GenericWith3Params<A, B, C>
    {
        
    }
    [Bean]
    internal class MultipleParamGenericUser
    {
        public GenericWith3Params<int, int, int> Multiple => multiple;
        [BeanReference]
        private GenericWith3Params<int, int, int> multiple;
    }

    [Bean]
    internal class NestedGeneric<T>
    {
        
    }
    [Bean]
    internal class WrapperGeneric<T>
    {
        
    }
    [Bean]
    internal class WrapperUser
    {
        public WrapperGeneric<NestedGeneric<int>> Nested => nested;
        [BeanReference]
        private WrapperGeneric<NestedGeneric<int>> nested;
    }
}