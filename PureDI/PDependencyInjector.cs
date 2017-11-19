using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using PureDI.Common;
using PureDI.Tree;
using static PureDI.Common.Common;

namespace PureDI
{

    /// <summary>
    /// The key class in the library.  This carries out the dependency injection
    /// </summary>
    /// <conceptualLink target="DI-Introduction">see Introduction</conceptualLink>
    [Bean]
    public partial class PDependencyInjector
    {
        /// <summary>
        /// caches the operating system in which the container is executing.
        /// Library users may have OS dependent injections
        /// </summary>
        public enum OS
        {
            /// <summary>
            /// Beans typically have an OS of OS.Any
            /// this will match any OS under which the container is executing
            /// </summary>
            Any,
            /// <summary>
            /// Any version of Linux supporting dotnetstandard 2.0
            /// </summary>
            Linux,
            /// <summary>
            /// Any version of Windows supporting dotnetstandard 2.0
            /// </summary>
            Windows,
            /// <summary>
            /// Any MAC version supported by dotnetstandard 2.0
            /// </summary>
            MacOS
        }
        private readonly OS os = new StdOSDetector().DetectOS();
        internal const string DEFAULT_PROFILE_ARG = "";
        internal const string DEFAULT_BEAN_NAME = "";
        internal const string DEFAULT_CONSTRUCTOR_NAME = "";

        private int multipleCallGuard;
        /// <summary>
        /// A parameter of this type can be passed to the constructor
        /// to indicate whether the default scanning of libraries is performed.
        /// </summary>
        /// <conceptualLink target="DI-Assemblies">See the Notes section of Assemblies</conceptualLink>
        [Flags]
        public enum AssemblyExclusion
        {
            /// <summary>
            /// default - the assembly containing the root type will
            /// be scanned for beans as will the PDependencyInjector library
            /// itself
            /// </summary>
            ExcludedNone = 0,
            /// <summary>
            /// The library itself should not be scanned for dependencies
            /// </summary>
            ExcludePDependencyInjector = 1,
            /// <summary>
            /// The assembly containing the type passed to CreateAndInjectDependencies
            /// will not be scanned for beans
            /// </summary>
            ExcludeRootTypeAssembly = 2
        }
        private readonly AssemblyExclusion excludedAssemblies;
        private readonly ImmutableArray<Assembly> explicitAssemblies;
        private readonly ISet<string> profileSet;

        /// <summary>
        /// this routine is called to specify the assemblies to be scanned
        /// for beans.  Any bean to be injected must be defined in one
        /// of these assemblies and must be marked with the [Bean] attribute.
        /// </summary>
        /// <remarks>
        /// The assembly containing SimpleIOCCBean class itself is always included
        /// by default.  It does not need to be specified.  The purpose
        /// of the inclusion is to allow callers to include the PDependencyInjector
        /// bean itself in factories.  The assembly is included to make this intuitive.
        /// </remarks>
        /// <example>SetAssemblies( true, "MyApp", "MyLib")</example>
        /// <param name="Profiles">See detailed description of profiles (See Also, below)</param>
        /// <param name="Assemblies">the assemblies to be scanned for injection</param>
        /// <param name="ExcludeAssemblies">flags to indicate which assemblies 
        /// (that would otherwise be automatically included in the scan) should be excluded</param>
        /// <onceptualLink target="DI-Profiles">See description of profiles</onceptualLink>
        public PDependencyInjector(string[] Profiles = null
          , Assembly[] Assemblies = null
          , AssemblyExclusion ExcludeAssemblies = AssemblyExclusion.ExcludedNone)
        {
            CheckProfilesArgument(Profiles);
            excludedAssemblies = ExcludeAssemblies;
            explicitAssemblies = (Assemblies != null
              ? ImmutableArray.Create<Assembly>(Assemblies)
              : ImmutableArray<Assembly>.Empty).ToImmutableArray();
            profileSet = new HashSet<string>(Profiles
              ?? new string[0], new CaseInsensitiveEqualityComparer());
        }

        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <typeparam name="TRootType">Typically, the root node of a tree of objects </typeparam>
        /// <param name="injectionState">This is null the first time the method is called.
        /// Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="rootBeanName">pass a bean name in the edge case when an interface
        /// or base class is passed as the root type but has multiple implementations</param>
        /// <param name="rootConstructorName">pass a constructor name in the edge case when 
        /// a class is being passed as the root type with multiple constructors</param>
        /// <param name="scope">See links below for an explanation of scope.  The scope passed in will apply to the 
        /// root bean only.  It has no effect on the rest of the tree.</param>
        /// <returns>an object of rootType</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (TRootType rootBean, InjectionState injectionState)
            CreateAndInjectDependencies<TRootType>(
                InjectionState injectionState = null, string rootBeanName = DEFAULT_BEAN_NAME
                , string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            (object rootObject, InjectionState newInjectionState)
                = CreateAndInjectDependencies(typeof(TRootType), injectionState
                , rootBeanName, rootConstructorName, scope);
            return ((TRootType) rootObject, newInjectionState);
        }
        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootType">Typically, the root node of a tree of objects </param>
        /// <param name="injectionState">This is null the first time the method is called.
        /// Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="rootBeanName">pass a bean name in the edge case when an interface
        /// or base class is passed as the root type but hs multiple implementations</param>
        /// <param name="rootConstructorName">pass a constructor name in the edge case when 
        /// a class is being passed as the root type with multiple constructors</param>
        /// <param name="scope">See links below for an explanation of scope.  The scope passed in will apply to the 
        /// root bean only.  It has no effect on the rest of the tree.</param>
        /// <returns>an object of rootType</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState)
          CreateAndInjectDependencies(Type rootType
            ,InjectionState injectionState = null, string rootBeanName = DEFAULT_BEAN_NAME
            , string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            CheckArgument(rootType);
            CheckInjectionStateArgument(injectionState);

            object rootObject;
            InjectionState newInjectionState = CloneOrCreateInjectionState(rootType, injectionState);
            (rootObject, newInjectionState) 
                = CreateAndInjectDependenciesExCommon(
                rootType, newInjectionState, rootBeanName, rootConstructorName, scope);
            return (rootObject, newInjectionState);
        }
        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootTypeName">Typically, the root node of a tree of objects </param>
        /// <param name="injectionState">This is null the first time the method is called.
        /// Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="rootBeanName">pass a bean name in the edge case when an interface
        /// or base class is passed as the root type but hs multiple implementations</param>
        /// <param name="rootConstructorName">pass a constructor name in the edge case when 
        /// a class is being passed as the root type with multiple constructors</param>
        /// <param name="scope">See links below for an explanation of scope.  The scope passed in will apply to the 
        /// root bean only.  It has no effect on the rest of the tree.</param>
        /// <returns>an object of rootType</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(
          string rootTypeName, InjectionState injectionState = null
            , string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME
            , BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(rootTypeName);
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            CheckInjectionStateArgument(injectionState);

            InjectionState newInjectionState = CloneOrCreateInjectionState(this.GetType(),injectionState);
            (Type rootType, string beanName) = newInjectionState.TypeMap.Keys.FirstOrDefault(k
              => AreTypeNamesEqualish(k.beanType.FullName, rootTypeName));
            if (rootType == null)
            {
                string allAssemblyNames
                    = ((dynamic)newInjectionState.Diagnostics.Groups[Constants.ASSEMBLIES_INFO].Occurrences[0]).Assemblies;
                Diagnostics.Group group = newInjectionState.Diagnostics.Groups["MissingRootBean"];
                dynamic diag = group.CreateDiagnostic();
                diag.BeanType = rootTypeName;
                diag.BeanName = rootBeanName;
                group.Add(diag);
                throw new DIException(
                  $"Unable to find a type in assembly {allAssemblyNames} for {rootTypeName}{Environment.NewLine}Remember to include the namespace"
                  , newInjectionState.Diagnostics);
            }
            return CreateAndInjectDependenciesExCommon(rootType
                , newInjectionState, rootBeanName
                , rootConstructorName, scope);
        }
        /// <summary>
        /// This version of the injection method allows the library user to instantiate an object
        /// using "new" or by whatever other means and have it injected into the object tree.
        /// 
        /// It may be appropriate to make one or more calls to create objects that will be needed in the tree
        /// before using a different call to create the full tree.
        /// 
        /// Currently this method will attempt to recursively inject dependencies for rootObject and
        /// this may not be the desired behaviour.  This is a shortcoming and will be addressed
        /// in a future version.
        /// </summary>
        /// <param name="rootObject">some instantiated object which the library user needs
        /// to attach to the object tree</param>
        /// <param name="injectionState">This is null the first time the method is called.
        /// Subsequent calls will typically take some saved instance of injection state.</param>
        /// <returns>an object which is the root of a tree of dependencies</returns>
        public (object rootBean, InjectionState injectionState) 
          CreateAndInjectDependencies(object rootObject
          , InjectionState injectionState = null)
        {
            CheckArgument(rootObject);
            CheckInjectionStateArgument(injectionState);

            InjectionState newInjectionState = CloneOrCreateInjectionState(rootObject.GetType(), injectionState);
            if ((excludedAssemblies & AssemblyExclusion.ExcludePDependencyInjector) == 0)
            {
                newInjectionState.MapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
            }
            if ((excludedAssemblies & AssemblyExclusion.ExcludeRootTypeAssembly) == 0)
            {
                newInjectionState.MapObjectsCreatedSoFar[(rootObject.GetType(), DEFAULT_BEAN_NAME)] = rootObject;
            }
            string profile = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            ObjectTree tree = new ObjectTree();
            newInjectionState = tree.CreateAndInjectDependencies(rootObject, newInjectionState);

            return (rootObject, newInjectionState);
        }
        /// <summary>
        /// <see cref="CreateAndInjectDependencies(string,InjectionState,string,string,BeanScope)"/>
        /// this overload does not print out the diagnostics
        /// </summary>
        /// <param name="rootType"></param>
        /// <param name="injectionState"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope"></param>
        private (object rootObject, InjectionState injectionState) CreateAndInjectDependenciesExCommon(Type rootType
          , InjectionState injectionState, string rootBeanName
          , string rootConstructorName, BeanScope scope)
        {
            IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap;
            IDictionary<(Type, string), object> mapObjectsCreatedSoFar;
            Diagnostics diagnostics;
            (diagnostics, typeMap, mapObjectsCreatedSoFar) = injectionState;
            string profileSetKey = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            if ((excludedAssemblies & AssemblyExclusion.ExcludePDependencyInjector) == 0)
            {
                injectionState.MapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
                // factories and possibly other beans may need access to the PDependencyInjector itself
                // so we include it as a bean by default
            }
            if (mapObjectsCreatedSoFar.ContainsKey((rootType, rootBeanName)))
            {
                return (mapObjectsCreatedSoFar[(rootType, rootBeanName)]
                    , new InjectionState(
                        injectionState.Diagnostics
                        , typeMap
                        , mapObjectsCreatedSoFar
                    ));
            }
            ObjectTree tree = new ObjectTree();
            object rootObject;
            (rootObject, injectionState) = tree.CreateAndInjectDependencies(
              rootType, injectionState, rootBeanName.ToLower(), rootConstructorName.ToLower(), scope, mapObjectsCreatedSoFar);
            if (rootObject == null && injectionState.Diagnostics.HasWarnings)
            {
                throw new DIException("Failed to create object tree - see diagnostics for details", injectionState.Diagnostics);
            }
            Assert(rootType.IsAssignableFrom(rootObject.GetType()));
            return (rootObject, injectionState);
#if false
            return (rootObject
                , new InjectionState(
                    injectionState.Diagnostics
                    , typeMap
                    , mapObjectsCreatedSoFar
                ));
#endif
        }
        private InjectionState CloneOrCreateInjectionState(Type rootType, InjectionState injectionState)
        {
            InjectionState newInjectionState;
            if (injectionState == null || injectionState.IsEmpty())
            {
                IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap;
                IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
                    new Dictionary<(Type, string), object>();
                Diagnostics diagnostics;
                (typeMap, diagnostics) = CreateTypeMap(rootType);
                newInjectionState = new InjectionState(diagnostics, typeMap, mapObjectsCreatedSoFar);
            }
            else
            {
                newInjectionState = CloneInjectionState(injectionState);
            }
            return newInjectionState;
        }

        private
            InjectionState CloneInjectionState(InjectionState injectionState)
        {
            return injectionState?.Clone() ?? InjectionState.Empty;
        }

        private (IWouldBeImmutableDictionary<(Type beanType, string beanName)
          , Type>, Diagnostics diagnostics)
          CreateTypeMap(Type rootType)
        {
            Diagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
            IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap = null;

            // make sure that the IOC Container itself is available as a bean
            // particularly to factories
            var builder = ImmutableList.CreateBuilder<Assembly>();
            builder.AddRange(explicitAssemblies
                .Union(new[] { rootType.Assembly }.Where(a =>
                     (excludedAssemblies & AssemblyExclusion.ExcludeRootTypeAssembly) == 0))
                    .Union(new[] { this.GetType().Assembly }.Where(a =>
                     (excludedAssemblies & AssemblyExclusion.ExcludePDependencyInjector) == 0)));
            IImmutableList<Assembly> allAssemblies = builder.ToImmutable();
            new BeanValidator().ValidateAssemblies(allAssemblies, diagnostics);
            if (typeMap == null)
            {
                typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(allAssemblies
                    , ref diagnostics, profileSet, os);
                LogAssemblies(diagnostics, allAssemblies);
                LogProfiles(diagnostics, profileSet);
                LogTypeMap(diagnostics, typeMap);
            }
            return (typeMap, diagnostics);
        }

        private void LogTypeMap(Diagnostics diagnostics
          , IDictionary<(Type beanType, string beanName), Type> types)
        {
            IEnumerable<(Type, string, Type)> typesQuery;
            if (types.Count == 0)
            {
                typesQuery = new(Type, string, Type)[] { (null, "none", null) };
            }
            else
            {
                typesQuery = types.Select(kv => (kv.Key.beanType, kv.Key.beanName, kv.Value)).OrderBy(kv => kv.beanType.FullName);
            }
            Diagnostics.Group group = diagnostics.Groups["TypesInfo"];
            foreach (var ti in typesQuery)
            {
                dynamic diag = group.CreateDiagnostic();
                diag.ReferenceType = ti.Item1?.FullName ?? "none";
                diag.BeanName = ti.Item2 ?? "none";
                diag.ImplementationType = ti.Item3?.FullName ?? "none";
                diagnostics.Groups["TypesInfo"].Occurrences.Add(diag);
            }
        }

        private void LogProfiles(Diagnostics diagnostics, ISet<string> profileSet)
        {
            Diagnostics.Group group = diagnostics.Groups["ProfilesInfo"];
            dynamic diag = group.CreateDiagnostic();
            diag.Profiles = string.Join(",", profileSet);
            diagnostics.Groups["ProfilesInfo"].Occurrences.Add(diag);
        }

        private void LogAssemblies(Diagnostics diagnostics
          , IImmutableList<Assembly> assemblies)
        {
            Diagnostics.Group group = diagnostics.Groups[Constants.ASSEMBLIES_INFO];
            dynamic diag = group.CreateDiagnostic();
            diag.Assemblies = string.Join(",", assemblies.Select(a => a.GetName().Name));
            diagnostics.Groups[Constants.ASSEMBLIES_INFO].Occurrences.Add(diag);
        }

        /// <summary>
        /// NOT USED: builds list of all the assemblies involved in the dependency tree
        /// </summary>
        /// <param name="assemblyNames">list of names provided by caller.
        ///     This should include the names of all assemblies containing dependencies (concrete classes)
        ///     involved in the object model of this app.  Optionally this can include the assembly
        ///     of the root class in the tree</param>
        /// <returns></returns>
        private IList<Assembly> AssembleAssemblies(IList<string> assemblyNames)
        {
            ISet<string> assemblyNameSet = assemblyNames.ToHashSet(s => s);
            return AppDomain.CurrentDomain.GetAssemblies()
              .Where(a => assemblyNameSet.Contains(a.GetName().Name)).ToList();
        }
        /// <param name="typeFullName">classic Type.FullName</param>
        /// <param name="IOCCUserEnteredName">Hopefully same as Type.FullName except for generics where
        ///     parameters have the same format as a program delcaration, e.g. MyClass&lt;MyOuterParam&lt;MyInnerParam&gt;&gt;</param>
        /// <returns>true if the types match i.e.
        ///     if a type identified by IOCCUserEnteredName would output typeFullName as its FullName</returns>
        private static bool AreTypeNamesEqualish(string typeFullName, string IOCCUserEnteredName)
        {
            return typeFullName == IOCCUserEnteredName;
        }
        private void CheckArgument(object o)
        {
            if (o == null)
            {
                throw new ArgumentNullException();
            }
        }
        private void CheckProfilesArgument(object[] o)
        {
            if (o == null)
            {
                return;
            }
            if (((string[])o).Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException();
            }
        }
        private void CheckInjectionStateArgument(InjectionState injectionState)
        {
            if (injectionState == null)
            {
                int previousValue = Interlocked.CompareExchange(ref multipleCallGuard, 1, 0);
                if (previousValue != 0)
                {
                    throw new ArgumentException(
                      "Operation failed: you are attempting to call CreateAndInjectDependencies for a second time"
                      + Environment.NewLine + "You must pass in the instance of injectionState returned by a previous call to CreateAndInjectDependencies");
                }
            }
        }


    }   // PDependencyInjector

    internal class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }

    /// <summary>
    /// The BeanScope enum is in
    /// conjunction with bean references to determine
    /// how multiple references to a particular bean will be
    /// dealt with.  Where the scope is Singleton (which is
    /// the default then all references with that
    /// scope will point to the same object.  Any
    /// reference with a scope of Prototype will point
    /// to separate object.
    /// </summary>
    public enum BeanScope
    {
        /// <summary>
        /// bean references with this scope (which is the default)
        /// will share an instance of the bean
        /// </summary>
        Singleton,
        /// <summary>
        /// bean references with this scope will be assigned 
        /// an unique instance of the bean class
        /// </summary>
        Prototype
    }

    internal static class IOCCLocalExtensions
    {
        public static string ListContents(this IList<string> assemblyNames, string separator = ", ")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(assemblyNames[0]);
            foreach (var name in assemblyNames.Skip(1))
            {
                sb.Append(separator);
                sb.Append(name);
            }
            return sb.ToString();
        }
        public static string ListContents(this IImmutableList<Assembly> assemblies, string separator = ", ")
        {
            IList<string> assemblyNames = assemblies.Select(a => a.GetName().Name).ToList();
            return ListContents(assemblyNames);
        }

        public static bool HasAFactory(this MemberInfo type)
        {
            return type.GetCustomAttributes().Any(ca => ca.GetType()
              == typeof(BeanReferenceBaseAttribute) &&
              (ca as BeanReferenceBaseAttribute).Factory != null);
        }

        public static BeanReferenceBaseAttribute GetBeanReferenceAttribute(this MemberInfo type)
        {
            return (BeanReferenceBaseAttribute)type.GetCustomAttributes().Where(
                ca => ca is BeanReferenceBaseAttribute).FirstOrDefault();
        }
        public static BeanReferenceBaseAttribute GetBeanReferenceAttribute(this ParameterInfo type)
        {
            return (BeanReferenceBaseAttribute)type.GetCustomAttributes().Where(
                ca => ca is BeanReferenceBaseAttribute).FirstOrDefault();
        }
    }
}
