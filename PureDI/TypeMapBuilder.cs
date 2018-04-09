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
            var wellFormedBeanSpecs
                = assemblies.SelectMany(a => a.GetTypes(), (a, t) => t).Where(
                d => d.TypeIsABean(profileSet, os)).SelectMany(d
                    => d.GetBaseClassesAndInterfaces().IncludeImplementation(d)
                        .Select(i => new BeanSpec(i, d.GetBeanName(), d)))
                        .OrderBy(bs => bs.RefId).ThenBy(bs => bs.Precendence)
                        ;
            var validBeanSpecs = wellFormedBeanSpecs.Where(bs => !bs.IsImplementationEnum 
                && !bs.IsImplementationStatic && !bs.IsImplementationAbstract).ToList();
            var bestFitBeanSpecs = validBeanSpecs.GroupBy(bs => bs.RefId).SelectMany(
                grp => grp.Where(bs2 => bs2.Precendence == grp.ElementAt(0).Precendence), (bs, bs2) => bs2).ToList();
            var groupedBestFitBeanSpecs = bestFitBeanSpecs.GroupBy(bs => bs.RefId).ToList();
            var dedupedBeanSpecs = groupedBestFitBeanSpecs.Select(grp => grp.ElementAt(0)).ToList();
            IDictionary<(Type, string), Type> map = dedupedBeanSpecs.ToDictionary(
                bs => (bs.ReferenceType, bs.BeanName), bs => bs.ImplementationType);
          
            var duplicateBeanSpecs = groupedBestFitBeanSpecs.SelectMany(
              grp => grp.Skip(1), (grp, bs2) => (grp.ElementAt(0), bs2)).ToList();
            LogDuplicateBeans(diagnostics, duplicateBeanSpecs);
            var invalidBeanSpecs = wellFormedBeanSpecs.Where(bs =>  
                bs.IsImplementationStatic || bs.IsImplementationAbstract);
                // not sure why we don't log enums as invalid beans
            LogInvalidBeans(diagnostics, invalidBeanSpecs);

            return (IReadOnlyDictionary<(Type, string), Type>)map;
        }

        private void LogDuplicateBeans(Diagnostics diagnostics, IEnumerable<(BeanSpec, BeanSpec)> duplicatePairs)
        {
            Diagnostics.Group group = diagnostics.Groups["DuplicateBean"];
            Diagnostic MakeInvalidBeanError(BeanSpec existingBeanSpec, BeanSpec rejectedBeanSpec)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.Interface1 = rejectedBeanSpec.ReferenceType.GetIOCCName();
                diag.BeanName = rejectedBeanSpec.BeanName;
                diag.NewBean = rejectedBeanSpec.ImplementationType.GetIOCCName();
                diag.ExistingBean = existingBeanSpec.ImplementationType.GetIOCCName(); // the other bean (in the duplicate)
                return diag;
            }
            var diagset = duplicatePairs.Select(dp => MakeInvalidBeanError(dp.Item1, dp.Item2));
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

            private readonly Type _referenceType;
            private readonly string _beanName;
            private readonly Type _implementationType;

            public BeanSpec(Type referenceType, string beanName, Type implementationType)
            {
                _referenceType = referenceType;
                _beanName = beanName;
                _implementationType = implementationType;
            }

            public Type ReferenceType => _referenceType;
            public string BeanName => _beanName;
            public Type ImplementationType => _implementationType;

            public bool IsImplementationStatic => _implementationType.IsStatic();
            public bool IsImplementationAbstract => _implementationType.IsAbstract;
            public bool IsImplementationEnum => _implementationType.IsValueType && ReferenceType != _implementationType;

            public int Precendence =>
                this._implementationType.GetBeanProfile() != Constants.DefaultProfileArg
                && this._implementationType.GetBeanOs() != Os.Any
                    ? 1
                    : this._implementationType.GetBeanProfile() != Constants.DefaultProfileArg
                        ? 2
                        : this._implementationType.GetBeanOs() != Os.Any
                            ? 3
                            : 4;

            public string RefId => _referenceType.FullName + BeanName;

        }       // BeanSpec     
    }           // TypeMapBuilder

    internal static class TypeMapBuilderExtensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> MyGroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            return Enumerable.GroupBy<TSource, TKey>(source, keySelector);
        }
         public static bool TypeIsABean(this Type type, ISet<string> profileSet, Os os)
        {
            BeanBaseAttribute ba 
              = (BeanBaseAttribute)type.GetCustomAttributes()
              .FirstOrDefault(attr => attr is BeanBaseAttribute);
            return 
              ba != null 
              && (
              ba.Profile == Constants.DefaultProfileArg
              || profileSet.Contains(ba.Profile))
              && (ba.OS == Os.Any 
              || ba.OS == os);
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
