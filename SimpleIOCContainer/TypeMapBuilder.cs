﻿using System;
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
                  = assembly.GetTypes().Where(d => d.TypeIsABean(profile, os)).SelectMany(d
                  => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                  .Select(i => ((i, d.GetBeanName()), d)));
                IList<((Type, string), Type)> list = query.ToList();
                foreach (((Type beanInterface, string name), Type beanImplementation) in query)
                {
                    if (beanImplementation.IsValueType && beanInterface != beanImplementation)
                    {
                        // this is a struct and dependencyInterface is System.ValueType which
                        // should be hidden, so we ignore it
                        continue;
                    }
                    if (beanImplementation.IsAbstract)
                    {
                        IOCCDiagnostics.Group group = diagnostics.Groups["InvalidBean"];
                        dynamic diag = group.CreateDiagnostic();
                        diag.AbstractClass = beanImplementation.GetIOCCName();
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
        public static bool TypeIsABean(this Type type, string profile, IOCC.OS os)
        {
            IOCCBeanAttribute ida 
              = (IOCCBeanAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is IOCCBeanAttribute);
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
            foreach (Type beanInterface in interfaces)
            {
                yield return beanInterface;
            }
        }

        public static string GetBeanName(this Type bean)
        {
            return bean.GetCustomAttributes<IOCCBeanAttribute>().Select(attr => attr.Name).FirstOrDefault();
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
