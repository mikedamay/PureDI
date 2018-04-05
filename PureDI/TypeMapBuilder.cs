using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PureDI.Attributes;
using PureDI.Common;
using PureDI.Public;
using static PureDI.Common.Common;

namespace PureDI
{
    internal class TypeMapBuilder
    {
        public IReadOnlyDictionary<(Type type, string name), Type> 
          BuildTypeMapFromAssemblies(IEnumerable<Assembly> assemblies
          , ref Diagnostics diagnostics, ISet<string> profileSet, Os os)
        {
            Dictionary<(Type, string), Type> map = new Dictionary<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var wellFormedBeanSpecs
                    = assembly.GetTypes().Where(d => d.TypeIsABean(profileSet, os)).SelectMany(d
                        => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                            .Select(i => new BeanSpec(i, d.GetBeanName(), d))).OrderBy(b => b.ImplementationType.FullName);
                    // only sorting so that unit testing is easier.
                Type beanInterface;
                string name;
                Type beanImplementation;
                var validBeanSpecs = wellFormedBeanSpecs.Where(bs => !bs.IsImplementationEnum 
                  && !bs.IsImplementationStatic && !bs.IsImplementationAbstract);
                var invalidBeanSpecs = wellFormedBeanSpecs.Where(bs =>  
                  bs.IsImplementationStatic || bs.IsImplementationAbstract);
                    // not sure why we don't log enums as invalid beans
                LogInvalidBeans(diagnostics, invalidBeanSpecs);


                foreach (BeanSpec beanSpec in validBeanSpecs)
                {
                    beanInterface = beanSpec.InterfaceType;
                    name = beanSpec.BeanName;
                    beanImplementation = beanSpec.ImplementationType;

                    if (map.ContainsKey((beanInterface, name)))
                    {
                        BestFit bestFit = GetBestDuplicate(map, (beanInterface, name), beanImplementation, profileSet);
                        if (bestFit == BestFit.Duplicate)
                        {
                            Diagnostics.Group group = diagnostics.Groups["DuplicateBean"];
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
            return map;
        }

        private void LogInvalidBeans(Diagnostics diagnostics, IEnumerable<BeanSpec> invalidBeanSpecs)
        {
            var diagset = invalidBeanSpecs.Select(bs => LogInvalidBeanError(diagnostics, bs
              , bs.IsImplementationAbstract ? "abstract": "static"));
            Diagnostics.Group group = diagnostics.Groups["InvalidBean"];
            foreach (dynamic diag in diagset)
            {
                group.Add(diag);
            }
        }

        private static Diagnostic LogInvalidBeanError(Diagnostics diagnostics, BeanSpec beanSpec, string errorCategory)
        {
            Diagnostics.Group group = diagnostics.Groups["InvalidBean"];
            dynamic diag = group.CreateDiagnostic();
            diag.AbstractOrStaticClass = beanSpec.ImplementationType.GetIOCCName();
            diag.ClassMode = errorCategory;
            return diag;
        }

        enum BestFit { Duplicate, NewItemBest, ExistingItemBest}
        // favor beans with a specific profile over those without a profile.
        private BestFit  GetBestDuplicate(IDictionary<(Type, string), Type> map
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

        private class BeanSpec
        {
            private readonly Type _interfaceType;
            private readonly string _beanName;
            private readonly Type _implementationType;
            public BeanSpec(Type interfaceType, string beanName, Type implementationType)
            {
                _interfaceType = interfaceType;
                _beanName = beanName;
                _implementationType = implementationType;
            }

            public Type InterfaceType => _interfaceType;
            public string BeanName => _beanName;
            public Type ImplementationType => _implementationType;

            public bool IsImplementationStatic => _implementationType.IsStatic();
            public bool IsImplementationAbstract => _implementationType.IsAbstract;
            public bool IsImplementationEnum => _implementationType.IsValueType && InterfaceType != _implementationType;
        }
    }

    internal static class TypeMapExtensions
    {
        public static bool TypeIsABean(this Type type, ISet<string> profileSet, Os os)
        {
            BeanBaseAttribute ida 
              = (BeanBaseAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is BeanBaseAttribute);
            return 
              ida != null 
              && (
              ida.Profile == Constants.DefaultProfileArg
              || profileSet.Contains(ida.Profile))
              && (ida.OS == Os.Any 
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

        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
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
