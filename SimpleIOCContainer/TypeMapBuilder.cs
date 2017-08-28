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
          , ref IOCCDiagnostics diagnostics, string profile, SimpleIOCContainer.OS os)
        {
            IDictionary<(Type, string), Type> map = new Dictionary<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var wellFormedBeanSpecs
                  = assembly.GetTypes().Where(d => d.TypeIsABean(profile, os)).SelectMany(d
                  => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                  .Select(i => ((i, d.GetBeanName()), d)));
                // 'inferred' names for tuple elements not supported by
                // .NET standard - apparently a 7.1 feature
                Type beanInterface;
                string name;
                Type beanImplementation;
                foreach (((Type beanInterfaceArg, string nameArg) beanInterfaceAndName, Type beanImplementationArg) beanId in wellFormedBeanSpecs)
                {
                    beanInterface = beanId.beanInterfaceAndName.beanInterfaceArg;
                    name = beanId.beanInterfaceAndName.nameArg;
                    beanImplementation = beanId.beanImplementationArg;

                    if (beanImplementation.IsValueType && beanInterface != beanImplementation)
                    {
                        // this is a struct and dependencyInterface is System.ValueType which
                        // should be hidden, so we ignore it
                        continue;
                    }
                    if (beanImplementation.IsAbstract && beanImplementation.IsSealed)
                    {
                        IOCCDiagnostics.Group group = diagnostics.Groups["InvalidBean"];
                        dynamic diag = group.CreateDiagnostic();
                        diag.AbstractOrStaticClass = beanImplementation.GetIOCCName();
                        diag.ClassMode = "static";
                        group.Add(diag);
                    }
                    else if (beanImplementation.IsAbstract)
                    {
                        IOCCDiagnostics.Group group = diagnostics.Groups["InvalidBean"];
                        dynamic diag = group.CreateDiagnostic();
                        diag.AbstractOrStaticClass = beanImplementation.GetIOCCName();
                        diag.ClassMode = "abstract";
                        group.Add(diag);
                    }
                    else
                    {
                        if (map.ContainsKey((beanInterface, name)))
                        {
                            IOCCDiagnostics.Group group = diagnostics.Groups["DuplicateBean"];
                            dynamic diag = group.CreateDiagnostic();
                            diag.Interface1 = beanInterface.GetIOCCName();
                            diag.BeanName = name;
                            diag.NewBean = beanImplementation.GetIOCCName();
                            diag.ExistingBean = (map[(beanInterface, name)] as Type).GetIOCCName();
                            group.Add(diag);
                            continue;
                        }
                        map.Add((beanInterface, name), beanImplementation);                        
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
        public static bool TypeIsABean(this Type type, string profile, SimpleIOCContainer.OS os)
        {
            BeanAttribute ida 
              = (BeanAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is BeanAttribute);
            return 
              ida != null 
              && (
              ida.Profile == SimpleIOCContainer.DEFAULT_PROFILE
              || profile == ida.Profile)
              && (ida.OS == SimpleIOCContainer.OS.Any 
              || ida.OS == os);
        }
        public static IEnumerable<Type> IncludeImplementation(this IEnumerable<Type> interfaces, Type implementation)
        {
            yield return implementation;
            foreach (Type beanInterface in interfaces)
            {
                yield return beanInterface;
            }
        }

        public static string GetBeanName(this Type bean)
        {
            return bean.GetCustomAttributes<BeanAttribute>().Select(attr => attr.Name).FirstOrDefault();
        }
        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type)
        {
            return type.BaseType == typeof(object)
                ? type.GetInterfaces().Where( ci => ci.GetCustomAttributes().Count() == 0 || ci.GetCustomAttributes().All(ca => !(ca is IOCCIgnoreAttribute)))
                : Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                    .Distinct().Where(ci => ci.GetCustomAttributes().Count() == 0 || ci.GetCustomAttributes().All(ca => !(ca is IOCCIgnoreAttribute)));
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
