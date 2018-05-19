using System;
using System.Collections.Generic;
using PureDI.Tree;

namespace PureDI.Common
{
    internal class InstantiatedObjectDecisionTable
    {
        public delegate void TypeMapHandler(Func<InjectionState, (Type, string)> typeMapUpdater, Type beanType, BeanSpec beanSpec, object bean);
        public delegate void MapObjectsCreatedSoFarHandler(IDictionary<InstantiatedBeanId, object> map, InstantiatedBeanId id, object bean);
        public delegate void DeferredInHandler(ISet<ConstructableBean> deferredAssignments, ConstructableBean constructableBean);
        public delegate void DeferredOutHandler(ISet<ConstructableBean> deferredAssignments, ConstructableBean constructableBean);

        private static IDictionary<Key, Entry> map = new Dictionary<Key, Entry>
        {
            {new Key(true, BeanScope.Singleton)
              ,new Entry
                {
                    TypeMapHandler = (typeMap, type, spec, bean) => { }
                    ,MapObjectsCreatedSoFarHandler = ((map, id, bean) => _ = id)
                    ,DeferredInHandler = (deferred, bean) => deferred.Add(bean)
                    ,DeferredOutHandler = (deferred, bean) => deferred.Add(bean)
                } 
            }
            ,{new Key(false, BeanScope.Singleton)
                ,new Entry
                {
                    TypeMapHandler = (typeMap, type, spec, bean) => _ = type
                    ,MapObjectsCreatedSoFarHandler = (map, id, bean) => _ = id
                    ,DeferredInHandler = (deferred, bean) => _ = bean
                    ,DeferredOutHandler = (deferred, bean) => _ = bean
                } 
            }
            ,{new Key(true, BeanScope.Prototype)
                ,new Entry
                {
                    TypeMapHandler = (typeMap, type, spec, bean) => _ = type
                    ,MapObjectsCreatedSoFarHandler = (map, id, bean) => _ = id
                    ,DeferredInHandler = (deferred, bean) => _ = bean
                    ,DeferredOutHandler = (deferred, bean) => _ = bean
                } 
            }
            ,{new Key(false, BeanScope.Prototype)
                ,new Entry
                {
                    TypeMapHandler = (typeMap, type, spec, bean) => _ = type
                    ,MapObjectsCreatedSoFarHandler = (map, id, bean) => _ = id
                    ,DeferredInHandler = (deferred, bean) => _ = bean
                    ,DeferredOutHandler = (deferred, bean) => _ = bean
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
        public void MaybeAddBeanToTypeMap(Func<InjectionState, (Type, string)> typeMap, Type beanType, BeanSpec beanSpec, object bean)
        {
            _entry.TypeMapHandler(typeMap, beanType, beanSpec, bean);
        }
        public void MaybeAddObjectToCreatedSoFarMap(IDictionary<InstantiatedBeanId, object> map, InstantiatedBeanId id, object bean)
        {
            _entry.MapObjectsCreatedSoFarHandler(map, id, bean);
        }

        public void MaybeAddDeferredIn(ISet<ConstructableBean> deferredAssignments, ConstructableBean constructableBean)
        {
            _entry.DeferredInHandler(deferredAssignments, constructableBean);
        }
        public void MaybeAddDeferredOut(ISet<ConstructableBean> deferredAssignments, ConstructableBean constructableBean)
        {
            _entry.DeferredOutHandler(deferredAssignments, constructableBean);
        }

    }

}