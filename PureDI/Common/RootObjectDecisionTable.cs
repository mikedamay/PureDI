using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using PureDI.Tree;

namespace PureDI.Common
{
    /// Internal Processing:
    /// objects are either created using new or (pointlessly) CreateAndInject.  They can be of Singleton or
    /// Prototype Scope and either defer injections into their members or allow them immediately.
    /// The table below shows for which input combinations the various internal tables (on InjectionState) are
    /// updated.
    /// TypeMap and MapSoFar refer to TypeMap and MapObjectsCreatedSoFar on injection state.
    /// Deferred In refers to InjectionState.CreationContext.BeansWithDeferredAssignments as passed to this
    /// overload.
    /// Deferred Out refers to InjectionState.CreationContext.BeansWithDeferredAssignments returned from this
    /// overload.  The expectation is that this will be used in a subsequent call to CreateAndInject where
    /// the recently created RootObject will be picked up and injected as a dependency for some other bean.
    /// (The Deferred In / Out nastiness is the price we pay to have a single route through ObjectTree.CreateObjectTree
    /// whilst restricting the complexity of the library user's view to the idea of InjectionState). 
    /// Creation Method        Defer Injections        Scope                                            TypeMap    MapSoFar    Deferred in    Deferred out
    /// new                    yes                     Singleton    handle cyclical dependencies        yes        yes         no             yes
    /// new                    no                      Singleton    normal creation                     yes        yes         yes            no
    /// new                    yes                     Prototype    pointless                           yes        no          no             no
    /// new                    no                      Prototype    not avaialble for injection         yes (guid) no          yes            no
    /// CreateAndInject        yes                     Singleton    pointless - duplicate injections    multiple calls to CreateAndInject
    /// CreateAndInject        no                      Singleton    pointless - duplicate injections    for the same object are considered
    /// CreateAndInject        yes                     Prototype    pointless - no injections           pointless and may result in
    /// CreateAndInject        no                      Prototype    pointless - duplicate injections    diagnostic warnings
    internal class RootObjectDecisionTable
    {
        private  delegate void Decide(Action updater);

        private static Decide Noop = updater => {};
        private static Decide Update = updater => updater();

        private static IDictionary<Key, Entry> map = new Dictionary<Key, Entry>
        {
            {new Key(true, BeanScope.Singleton)
              ,new Entry
                {
                    TypeMapHandler = Update
                    ,MapObjectsCreatedSoFarHandler = Update
                    ,DeferredInHandler = Noop
                    ,DeferredOutHandler = Update
                } 
            }
            ,{new Key(false, BeanScope.Singleton)
                ,new Entry
                {
                    TypeMapHandler = Update
                    ,MapObjectsCreatedSoFarHandler = Update
                    ,DeferredInHandler = Update
                    ,DeferredOutHandler = Noop
                } 
            }
            ,{new Key(true, BeanScope.Prototype)
                ,new Entry
                {
                    TypeMapHandler = Update
                    ,MapObjectsCreatedSoFarHandler = Noop
                    ,DeferredInHandler = Noop
                    ,DeferredOutHandler = Noop
                } 
            }
            ,{new Key(false, BeanScope.Prototype)
                ,new Entry
                {
                    TypeMapHandler = Update
                    ,MapObjectsCreatedSoFarHandler = Noop
                    ,DeferredInHandler = Update
                    ,DeferredOutHandler = Noop
                } 
            }
        };
        private class Entry
        {
            public Decide TypeMapHandler;
            public Decide MapObjectsCreatedSoFarHandler;
            public Decide DeferredInHandler;
            public Decide DeferredOutHandler;
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
        public RootObjectDecisionTable(bool deferred, BeanScope beanScope)
        {
            _entry = map[new Key(deferred, beanScope)];
        }
        public void MaybeAddBeanToTypeMap(Action updater)
        {
            _entry.TypeMapHandler(updater);
        }
        public void MaybeAddObjectToCreatedSoFarMap(Action updater)
        {
            _entry.MapObjectsCreatedSoFarHandler(updater);
        }

        public void MaybeAddDeferredIn(Action updater)
        {
            _entry.DeferredInHandler(updater);
        }
        public void MaybeAddDeferredOut(Action updater)
        {
            _entry.DeferredOutHandler(updater);
        }

    }

}