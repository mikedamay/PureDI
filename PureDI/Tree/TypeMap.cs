using System;
using System.Collections.Generic;

namespace PureDI.Tree
{
    internal interface TypeMap : IReadOnlyDictionary<(Type beanType, string beanName), Type>
    {
        IDictionary<(Type beanType, string beanName), Type> GetDictionary();
    }
    internal class TypeMapImpl : Dictionary<(Type beanType, string beanName), Type>, TypeMap
    {
        public TypeMapImpl(IReadOnlyDictionary<(Type beanType, string beanName) , Type> map)
        {
            foreach (var entry in map)
            {
                base.Add(entry.Key, entry.Value );
            }           
        }

        public IDictionary<(Type beanType, string beanName), Type> 
            GetDictionary()
        {
            return this;
        }
    }        
/*
    internal interface TypeMap : IReadOnlyDictionary<BeanSpec, Type>
    {
        
    }
    internal class TypeMapImpl : Dictionary<BeanSpec, Type>, TypeMap
    {
        public TypeMapImpl(IReadOnlyDictionary<(Type type, string beanName, string constructorName), Type> map)
        {
            foreach (var entry in map)
            {
                base.Add(new BeanSpec(entry.Key.type, entry.Key.beanName, entry.Key.constructorName), entry.Value );
            }           
        }
    }        
*/
}