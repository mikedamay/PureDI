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
                            .Select(i => new BeanSpec(i, d.GetBeanName(), d))).OrderBy(bs => bs)
                            ;
                Type beanInterface;
                string name;
                Type beanImplementation;
                var validBeanSpecs = wellFormedBeanSpecs.Where(bs => !bs.IsImplementationEnum 
                  && !bs.IsImplementationStatic && !bs.IsImplementationAbstract);
                var invalidBeanSpecs = wellFormedBeanSpecs.Where(bs =>  
                  bs.IsImplementationStatic || bs.IsImplementationAbstract);
                    // not sure why we don't log enums as invalid beans
                var beanSpecComparisons = validBeanSpecs.SelectMany(
                    bs1 => validBeanSpecs.Where(bs2 => bs1.InterfaceMatches(bs2))
                    .OrderBy(bs2 => bs2.Precendence).Take(1)
                    , (bs1, bs2) => new {bs1, bs2});
                var bestFitBeanSpecs = beanSpecComparisons.Where(t 
                    => t.bs1.Precendence == t.bs2.Precendence).Select(t => t.bs1);
                var poorFitBeanSpecs = beanSpecComparisons.Where(t 
                    => t.bs1.Precendence != t.bs2.Precendence).Select(t => t.bs1);
                var dedupedBeanSpecs = bestFitBeanSpecs.GroupBy(bs => bs ).SelectMany(
                        grp => bestFitBeanSpecs.Where(bs2 => grp.Key.ImplementationType == bs2.ImplementationType)
                        , (grp, bs2) => new {bs1 = grp.Key, bs2})
                    .Select(t => t.bs1);
                var duplicateBeanSpecs = bestFitBeanSpecs.GroupBy(bs => bs ).SelectMany(
                        grp => bestFitBeanSpecs.Where(bs2 => grp.Key.ImplementationType != bs2.ImplementationType)
                        , (grp, bs2) => new {bs1 = grp.Key, bs2})
                    .Select(t => t.bs2);
                //map = dedupedBeanSpecs.ToDictionary(bs => (bs.InterfaceType, bs.BeanName), bs => bs.ImplementationType);
                
                LogInvalidBeans(diagnostics, invalidBeanSpecs);
                LogPoorFitBeans(diagnostics, poorFitBeanSpecs);
                LogDuplicateBeans(diagnostics, duplicateBeanSpecs);

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

        private void LogDuplicateBeans(Diagnostics diagnostics, IEnumerable<BeanSpec> duplicateBeanSpecs)
        {
            Diagnostics.Group group = diagnostics.Groups["DuplicateBean"];
            Diagnostic MakeInvalidBeanError(BeanSpec beanSpec)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.Interface1 = beanSpec.InterfaceType.GetIOCCName();
                diag.BeanName = beanSpec.BeanName;
                diag.NewBean = beanSpec.ImplementationType.GetIOCCName();
                diag.ExistingBean = ""; // the other bean (in the duplicate)
                return diag;
            }
            var diagset = duplicateBeanSpecs.Select(bs => MakeInvalidBeanError(bs));
            foreach (dynamic diag in diagset)
            {
                group.Add(diag);
            }
        }

        private void LogPoorFitBeans(Diagnostics diagnostics, IEnumerable<BeanSpec> poorFitBeanSpecs)
        {
            

        }

        private void LogInvalidBeans(Diagnostics diagnostics, IEnumerable<BeanSpec> invalidBeanSpecs)
        {
           Diagnostics.Group group = diagnostics.Groups["InvalidBean"];
           Diagnostic MakeInvalidBeanError(BeanSpec beanSpec, string errorCategory)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.AbstractOrStaticClass = beanSpec.ImplementationType.GetIOCCName();
                diag.ClassMode = errorCategory;
                return diag;
            }
            var diagset = invalidBeanSpecs.Select(bs => MakeInvalidBeanError(bs
              , bs.IsImplementationAbstract ? "abstract": "static"));
            foreach (dynamic diag in diagset)
            {
                group.Add(diag);
            }
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

        private class BeanSpec : IComparable<BeanSpec>
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

            public bool InterfaceMatches(BeanSpec bs2)
            {
                return this._interfaceType == bs2._interfaceType
                       && this._beanName == bs2._beanName;
            }

            public int Precendence =>
                this._implementationType.GetBeanProfile() != Constants.DefaultProfileArg
                && this._implementationType.GetBeanOs() != Os.Any
                    ? 1
                    : this._implementationType.GetBeanProfile() != Constants.DefaultProfileArg
                        ? 2
                        : this._implementationType.GetBeanOs() != Os.Any
                            ? 3
                            : 4;

            public string _beanSpecId => _interfaceType.FullName + BeanName;

            protected bool Equals(BeanSpec other)
            {
                return string.Equals(_beanSpecId, other._beanSpecId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((BeanSpec) obj);
            }

            public override int GetHashCode()
            {
                return (_beanSpecId != null ? _beanSpecId.GetHashCode() : 0);
            }

            public int CompareTo(BeanSpec other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return string.Compare(_beanSpecId, other._beanSpecId, StringComparison.Ordinal);
            }
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
        public static Os GetBeanOs(this Type bean)
        {
            return bean.GetCustomAttributes<BeanBaseAttribute>().Select(attr => attr.OS).FirstOrDefault();
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
