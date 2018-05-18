using System;
using System.Collections.Generic;
using PureDI.Tree;

namespace PureDI.Common
{
    internal class InstantiatedObjectDecisionTable
    {
        public delegate void TypeMapHandler(Type beanType, BeanSpec beanSpec, object bean);
        public delegate void MapObjectsCreatedSoFarHandler(InstantiatedBeanId id, object bean);
        public delegate void DeferredInHandler(ConstructableBean constructableBean);
        public delegate void DeferredOutHandler(ConstructableBean constructableBean);

        private static IDictionary<Key, Entry> map = new Dictionary<Key, Entry>
        {
            {new Key(true, BeanScope.Singleton)
              ,new Entry
                {
                    TypeMapHandler = ((type, spec, bean) => _ = type)
                } 
            }
            ,{new Key(false, BeanScope.Singleton)
                ,new Entry
                {
                    MapObjectsCreatedSoFarHandler = ((id, bean) => _ = id)
                } 
            }
            ,{new Key(true, BeanScope.Prototype)
                ,new Entry
                {
                    DeferredInHandler = (bean => _ = bean)
                } 
            }
            ,{new Key(false, BeanScope.Prototype)
                ,new Entry
                {
                    DeferredOutHandler = (bean => _ = bean)
                } 
            }
        };
        private class Entry
        {
            public TypeMapHandler TypeMapHandler;
            public MapObjectsCreatedSoFarHandler MapObjectsCreatedSoFarHandler;
            public DeferredInHandler DeferredInHandler;
            public DeferredOutHandler DeferredOutHandler;
        }

        private class Key
        {
            private readonly bool _deferredInjection;
            private readonly BeanScope _beanScope;

            public Key(bool deferredInjection, BeanScope beanScope)
            {
                _deferredInjection = deferredInjection;
                _beanScope = beanScope;
            }

            protected bool Equals(Key other)
            {
                return _deferredInjection == other._deferredInjection && _beanScope == other._beanScope;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Key) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_deferredInjection.GetHashCode() * 397) ^ (int) _beanScope;
                }
            }
        }

        private Entry _entry;
        public InstantiatedObjectDecisionTable(bool deferred, BeanScope beanScope)
        {
            _entry = map[new Key(deferred, beanScope)];
        }
        public void MaybeAddBeanToTypeMap(Type beanType, BeanSpec beanSpec, object bean)
        {
            _entry.TypeMapHandler(beanType, beanSpec, bean);
        }
        public void MaybeAddObjectToCreatedSoFarMap(InstantiatedBeanId id, object bean)
        {
            _entry.MapObjectsCreatedSoFarHandler(id, bean);
        }

        public void MaybeAddDeferredIn(ConstructableBean constructableBean)
        {
            _entry.DeferredInHandler(constructableBean);
        }
        public void MaybeAddDeferredOut(ConstructableBean constructableBean)
        {
            _entry.DeferredOutHandler(constructableBean);
        }

    }

}