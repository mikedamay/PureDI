using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class TypeMapBuilder
    {
        public IDictionary<(Type type, string name), TypeHolder> BuildTypeMapFromAssemblies(
          IEnumerable<Assembly> assemblies)
        {
            IDictionary<(Type, string), TypeHolder> map = new Dictionary<(Type, string), TypeHolder>();
            foreach (Assembly assembly in assemblies)
            {
                var query
                  = assembly.GetTypes().Where(TypeIsADependency).SelectMany(d 
                  => d.GetInterfaces().IncludeImplementation(d).Select(i => ((i, ""), d)));
                foreach (((Type dependencyImplementation, string name), Type dependencyInterface) in query)
                {
                    map.Add((dependencyInterface, name), new TypeHolder(dependencyImplementation));
                }
            }
            return map;
        }

        private bool TypeIsADependency(Type type)
        {
            return type.GetCustomAttributes().Any(attr => attr is IOCCDependencyAttribute);
        }
    }

    internal static class TypeMapExtensions
    {
        public static IEnumerable<Type> IncludeImplementation(this IEnumerable<Type> interfaces, Type implementation)
        {
            yield return implementation;
            foreach (Type dependencyInterface in interfaces)
            {
                yield return dependencyInterface;
            }
        }
    }

    internal class TypeHolder
    {
        public TypeHolder(Type type)
        {
            typeOrFactoryMethod = type;
        }
        private readonly object typeOrFactoryMethod;
        public object Content => typeOrFactoryMethod;
    }
}
