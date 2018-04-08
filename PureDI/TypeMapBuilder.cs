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
            IDictionary<(Type, string), Type> map = new Dictionary<(Type, string), Type>();
            foreach (Assembly assembly in assemblies)
            {
                var wellFormedBeanSpecs
                    = assembly.GetTypes().Where(d => d.TypeIsABean(profileSet, os)).SelectMany(d
                        => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                            .Select(i => new BeanSpec(i, d.GetBeanName(), d))).OrderBy(bs => bs.RefId).ThenBy(bs => bs.Precendence)
                            ;
                var validBeanSpecs = wellFormedBeanSpecs.Where(bs => !bs.IsImplementationEnum 
                  && !bs.IsImplementationStatic && !bs.IsImplementationAbstract).ToList();
                var invalidBeanSpecs = wellFormedBeanSpecs.Where(bs =>  
                  bs.IsImplementationStatic || bs.IsImplementationAbstract);
                    // not sure why we don't log enums as invalid beans
                //var beanSpecComparisons = validBeanSpecs.SelectMany(
                //    bs1 => validBeanSpecs.Where(bs2 => bs1.RefId == bs2.RefId)
                //    .OrderBy(bs2 => bs2.Precendence).Take(1)
                //    , (bs1, bs2) => new {bs1, bs2}).ToList();
                //var bestFitBeanSpecs2 = beanSpecComparisons.Where(t 
                //    => t.bs1.Precendence == t.bs2.Precendence).Select(t => t.bs1).ToList();
                var bestFitBeanSpecs = validBeanSpecs.GroupBy(bs => bs.RefId).SelectMany(
                    grp => grp.Where(bs2 => bs2.Precendence == grp.ElementAt(0).Precendence), (bs, bs2) => bs2).ToList();
                var groupedBestFitBeanSpecs = bestFitBeanSpecs.GroupBy(bs => bs.RefId).ToList();
                var dedupedBeanSpecs = groupedBestFitBeanSpecs.Select(grp => grp.ElementAt(0)).ToList();
                var duplicateBeanSpecs = groupedBestFitBeanSpecs.SelectMany(grp => grp.Skip(1)).ToList();
                IDictionary<(Type, string), Type> mapAssembly = dedupedBeanSpecs.ToDictionary(bs => (bs.InterfaceType, bs.BeanName), bs => bs.ImplementationType);
                map = new Dictionary<(Type, string), Type>( map.Concat(mapAssembly));
          
                LogDuplicateBeans(diagnostics, duplicateBeanSpecs);
                LogInvalidBeans(diagnostics, invalidBeanSpecs);
            }
            return (IReadOnlyDictionary<(Type, string), Type>)map;
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

            public string RefId => _interfaceType.FullName + BeanName;

        }       // BeanSpec     
    }           // TypeMapBuilder

    internal static class TypeMapExtensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> MyGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            return Enumerable.GroupBy<TSource, TKey>(source, keySelector);
        }
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
    }       // TypeMapExtensions
}           // PureDI
