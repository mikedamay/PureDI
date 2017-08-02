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
                  = assembly.GetTypes().Where(d => d.TypeIsADependency()).SelectMany(d
                  => d.GetAncestors().IncludeImplementation(d).Select(i => ((i, d.GetDependencyName()), d)));
                foreach (((Type dependencyInterface, string name), Type dependencyImplementation) in query)
                {
                    if (!dependencyImplementation.IsClass)
                    {
                        LogWarning($"{dependencyImplementation.Name} is not a class");
                    }
                    else
                    {
                        map.Add((dependencyInterface, name), new TypeHolder(dependencyImplementation));
                        
                    }
                }
            }
            return map;
        }

        private void LogWarning(string s)
        {
            
        }

    }

    internal static class TypeMapExtensions
    {
        public static bool TypeIsADependency(this Type type)
        {
            return type.GetCustomAttributes().Any(attr => attr is IOCCDependencyAttribute);
        }
        public static IEnumerable<Type> IncludeImplementation(this IEnumerable<Type> interfaces, Type implementation)
        {
            yield return implementation;
            foreach (Type dependencyInterface in interfaces)
            {
                yield return dependencyInterface;
            }
        }

        public static string GetDependencyName(this Type dependency)
        {
            return dependency.GetCustomAttributes<IOCCDependencyAttribute>().Select(attr => attr.Name).FirstOrDefault();
        }

        public static IEnumerable<Type> GetAncestors(this Type dependency)
        {
                foreach (Type dependencyInterface in dependency.GetInterfaces())
                {
                    foreach (Type remoteAncestor in dependencyInterface.GetInterfaces())
                    {
                        yield return remoteAncestor;
                    }              
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
