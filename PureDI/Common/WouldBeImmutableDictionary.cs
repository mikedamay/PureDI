﻿using System.Collections.Generic;

namespace PureDI.Common
{

    /// <summary>
    /// ImmutableDictionary was too slow to be of use.
    /// </summary>
    internal interface IWouldBeImmutableDictionary<K, V> : IDictionary<K, V>
    {
        
    }

    internal class WouldBeImmutableDictionary
    {
        public static WouldBeImmutableDictionary<K, V>.Builder CreateBuilder<K, V>()
        {
            return new WouldBeImmutableDictionary<K, V>.Builder();
        }

    }

    internal class WouldBeImmutableDictionary<K, V> : Dictionary<K, V>, IWouldBeImmutableDictionary<K, V>
    {
        internal class Builder : WouldBeImmutableDictionary<K, V>
        {
            public WouldBeImmutableDictionary<K, V> ToImmutable()
            {
                return this;
            }
        }

        internal WouldBeImmutableDictionary()
        {
            
        }
        internal WouldBeImmutableDictionary(IWouldBeImmutableDictionary<K, V> other) : base(other)
        {
            
        }
    }
}