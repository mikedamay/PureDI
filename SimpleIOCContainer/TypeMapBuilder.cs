using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class TypeMapBuilder
    {
        public IDictionary<(Type type, string name), Type> 
          BuildTypeMapFromAssemblies(IEnumerable<Assembly> assemblies
          , ref IOCCDiagnostics diagnostics, string profile, IOCC.OS os)
        {
            IDictionary<(Type, string), Type> map = new Dictionary<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var query
                  = assembly.GetTypes().Where(d => d.TypeIsADependency(profile, os)).SelectMany(d
                  => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                  .Select(i => ((i, d.GetDependencyName()), d)));
                IList<((Type, string), Type)> list = query.ToList();
                foreach (((Type dependencyInterface, string name), Type dependencyImplementation) in query)
                {
                    if (dependencyImplementation.IsAbstract)
                    {
                        IOCCDiagnostics.Group group = diagnostics.Groups["InvalidBean"];
                        dynamic diag = group.CreateDiagnostic();
                        diag.AbstractClass = dependencyImplementation.FullName;
                        group.Add(diag);
                    }
                    else
                    {
                        if (map.ContainsKey((dependencyInterface, name)))
                        {
                            IOCCDiagnostics.Group group = diagnostics.Groups["DuplicateBean"];
                            dynamic diag = group.CreateDiagnostic();
                            diag.Interface1 = dependencyInterface.FullName;
                            diag.BeanName = name;
                            diag.NewBean = dependencyImplementation.FullName;
                            diag.ExistingBean = (map[(dependencyInterface, name)] as Type).FullName;
                            group.Add(diag);
                            continue;
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
        public static bool TypeIsADependency(this Type type, string profile, IOCC.OS os)
        {
            IOCCDependencyAttribute ida 
              = (IOCCDependencyAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is IOCCDependencyAttribute);
            return 
              ida != null 
              && (
              ida.Profile == IOCC.DEFAULT_PROFILE
              || profile == ida.Profile)
              && (ida.OS == IOCC.OS.Any 
              || ida.OS == os);
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
