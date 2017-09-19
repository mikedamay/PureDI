using System.Collections.Generic;

namespace com.TheDisappointedProgrammer.IOCC.Common
{

    /// <summary>
    /// ImmutableDictionary was too slow to be of use.
    /// </summary>
    public interface IWouldBeImmutableDictionary<K, V> : IDictionary<K, V>
    {
        
    }

    public class WouldBeImmutableDictionary
    {
        public static WouldBeImmutableDictionary<K, V>.Builder CreateBuilder<K, V>()
        {
            return new WouldBeImmutableDictionary<K, V>.Builder();
        }

    }

    public class WouldBeImmutableDictionary<K, V> : Dictionary<K, V>, IWouldBeImmutableDictionary<K, V>
    {
        public class Builder : WouldBeImmutableDictionary<K, V>
        {
            public WouldBeImmutableDictionary<K, V> ToImmutable()
            {
                return this;
            }
        }

    }
}