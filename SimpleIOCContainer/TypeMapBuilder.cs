using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using static com.TheDisappointedProgrammer.IOCC.Common.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class TypeMapBuilder
    {
        public IImmutableDictionary<(Type type, string name), Type> 
          BuildTypeMapFromAssemblies(IEnumerable<Assembly> assemblies
          , ref IOCCDiagnostics diagnostics, ISet<string> profileSet, SimpleIOCContainer.OS os)
        {
            ImmutableDictionary<(Type, string), Type>.Builder map 
              = ImmutableDictionary.CreateBuilder<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var wellFormedBeanSpecs
                  = assembly.GetTypes().Where(d => d.TypeIsABean(profileSet, os)).SelectMany(d
                  => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                  .Select(i => ((i, d.GetBeanName()), d))).OrderBy(b => b.Item2.FullName);
                        // only sorting so that testing will cover a couple of tricky branches.
                // 'inferred' names for tuple elements not supported by
                // .NET standard - apparently a 7.1 feature - it doesn't seem to work for me
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
                            BestFit bestFit = QueryRemoveDuplicate(map, (beanInterface, name), beanImplementation, profileSet);
                            if (bestFit == BestFit.Duplicate)
                            {
                                IOCCDiagnostics.Group group = diagnostics.Groups["DuplicateBean"];
                                dynamic diag = group.CreateDiagnostic();
                                diag.Interface1 = beanInterface.GetIOCCName();
                                diag.BeanName = name;
                                diag.NewBean = beanImplementation.GetIOCCName();
                                diag.ExistingBean = (map[(beanInterface, name)] as Type).GetIOCCName();
                                group.Add(diag);
                                continue;       // don't add the new one - we're in a mess, don't make it worse
                            }
                            else if (bestFit == BestFit.NewItemBest)
                            {
                                map.Remove((beanInterface, name));  // get rid of this so we can add the new one
                            }
                            else if (bestFit == BestFit.ExistingItemBest)
                            {
                                continue;   // don't add the new one - we're already ok.
                            }
                        }
                        map.Add((beanInterface, name), beanImplementation);                        
                    }
                }
            }
            return map.ToImmutable();
        }
        enum BestFit { Duplicate, NewItemBest, ExistingItemBest}
        private BestFit QueryRemoveDuplicate(IDictionary<(Type, string), Type> map
          , (Type beanInterface, string name) beanId, Type beanImplementation, ISet<string> profileSet)
        {
            Assert(map.ContainsKey(beanId));
            Type existingType = map[beanId];
            string existingProfile = existingType.GetBeanProfile();
            string newProfile = beanImplementation.GetBeanProfile();
            bool existingProfileMatch = profileSet.Contains(existingProfile);
            bool newProfileMatch = profileSet.Contains(newProfile);
            if (profileSet.Count == 0)
            {
                // no profile has been specified
                return BestFit.Duplicate;
            }
            else if (existingProfileMatch && newProfileMatch)
            {
                // both candidates are consistent with profile
                return BestFit.Duplicate;
            }
            else if (newProfileMatch)
            {
                return BestFit.NewItemBest;
            }
            else // if (existingProfileMatch
            {
                return BestFit.ExistingItemBest;
            }
        }

        private void LogWarning(string s)
        {
            
        }

    }

    internal static class TypeMapExtensions
    {
        public static bool TypeIsABean(this Type type, ISet<string> profileSet, SimpleIOCContainer.OS os)
        {
            BeanBaseAttribute ida 
              = (BeanBaseAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is BeanBaseAttribute);
            return 
              ida != null 
              && (
              ida.Profile == SimpleIOCContainer.DEFAULT_PROFILE_ARG
              || profileSet.Contains(ida.Profile))
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
            return bean.GetCustomAttributes<BeanBaseAttribute>().Select(attr => attr.Name).FirstOrDefault();
        }
        public static string GetBeanProfile(this Type bean)
        {
            return bean.GetCustomAttributes<BeanBaseAttribute>().Select(attr => attr.Profile).FirstOrDefault();
        }
        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type)
        {
            return type.BaseType == typeof(object)
                ? type.GetInterfaces().Where( ci => ci.GetCustomAttributes().Count() == 0 || ci.GetCustomAttributes().All(ca => !(ca is IgnoreBaseAttribute)))
                : Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                    .Distinct().Where(ci => ci.GetCustomAttributes().Count() == 0 || ci.GetCustomAttributes().All(ca => !(ca is IgnoreBaseAttribute)));
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
