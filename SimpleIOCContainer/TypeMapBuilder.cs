using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class TypeMapBuilder
    {
        public IDictionary<(Type type, string name), Type> 
          BuildTypeMapFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            IDictionary<(Type, string), Type> map = new Dictionary<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var query
                  = assembly.GetTypes().Where(d => d.TypeIsADependency()).SelectMany(d
                  => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                  .Select(i => ((i, d.GetDependencyName()), d)));
                IList<((Type, string), Type)> list = query.ToList();
                foreach (((Type dependencyInterface, string name), Type dependencyImplementation) in query)
                {
                    if (!dependencyImplementation.IsClass)
                    {
                        LogWarning($"{dependencyImplementation.Name} is not a class");
                    }
                    else
                    {
                        if (map.ContainsKey((dependencyInterface, name)))
                        {
                            throw new Exception(
                              $"attempt to add duplicate dependency {(dependencyInterface.FullName, name)}"
                              + Environment.NewLine
                              + $"attempting to add ${dependencyImplementation.FullName}"
                              + Environment.NewLine
                              + $"when ${(map[(dependencyInterface, name)] as Type).FullName} is already included");
                        }
                        map.Add((dependencyInterface, name), dependencyImplementation);
                        
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
        /// <summary>
        /// returns all base classes and interfaces from which a class inherits
        /// either directly or indirectly
        /// </summary>
        /// <param name="dependency">typically a class marked as [IOCCDependency] but can be any class</param>
        /// <returns>all direct and indirect base classes and interfaces</returns>
        public static IEnumerable<Type> GetAncestors(this Type dependency)
        {
                foreach (Type dependencyInterface in dependency.GetInterfaces())
                {
                    yield return dependencyInterface;
                    GetAncestors(dependencyInterface);
                }
        }
        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type)
        {
            return type.BaseType == typeof(object)
                ? type.GetInterfaces()
                : Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                    .Distinct();
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
