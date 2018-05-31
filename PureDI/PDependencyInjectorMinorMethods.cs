using System;

namespace PureDI
{
    public partial class DependencyInjector
    {
        public object GetBean(Type classOrInterface, string beanName)
        {
            if (mapObjectsCreatedSoFar.ContainsKey((classOrInterface, beanName)))
            {
                return mapObjectsCreatedSoFar[(classOrInterface, beanName)];
            }
            return null;
        }

        public bool IsBeanInstantiated(Type classOrInterface, string beanName)
        {
            return GetBean(classOrInterface, beanName) != null;
        }
        // mention profiles
        public bool HasBeenDefinition(Type classOrInterface, string beanName)
        {
            if (typeMap == null)
            {
                return false;
            }
            return typeMap.ContainsKey((classOrInterface, beanName));
        }

    }
}