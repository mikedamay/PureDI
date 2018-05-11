using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using PureDI.Attributes;
using PureDI.Common;
using PureDI.Public;
using PureDI.Tree;
using static PureDI.Common.Common;

namespace PureDI
{
    /// <summary>
    /// The key class in the library.  This carries out the dependency injection
    /// </summary>
    /// <conceptualLink target="DI-Introduction">see Introduction</conceptualLink>
    [Bean]
    public class PDependencyInjector
    {
        private readonly Os os = new StdOSDetector().DetectOS();
        private readonly ISet<string> _profileSet;
        private readonly Boolean _ignoreRootTypeAssembly;
        private int _multipleCallGuard;

        /// <summary>
        /// Typically a single dependency injector is created, more often than not, at program startup
        /// </summary>
        /// <remarks>
        /// The dependency injector is created with an optional set of profiles which
        /// determine which beans are included in the dependency tree (e.g. test 
        /// objects rather than production objects).
        /// </remarks>
        /// <example>SetAssemblies( new string[] {"test"}, false)</example>
        /// <param name="profiles">See detailed description of profiles (See Also, below)</param>
        /// <param name="ignoreRootTypeAssembly">if true then the assembly of the
        /// root type passed to CreateAndInjectDependencies will not be included
        /// when scanning for dependencies - this is available to support testing
        /// during the development of this library</param>
        /// <onceptualLink target="DI-Profiles">See description of profiles</onceptualLink>
        public PDependencyInjector(string[] profiles = null, bool ignoreRootTypeAssembly = false
          )
        {
            CheckNoBlankProfiles(profiles);
            _profileSet = new HashSet<string>(profiles
              ?? new string[0], new CaseInsensitiveEqualityComparer());
            _ignoreRootTypeAssembly = ignoreRootTypeAssembly;
        }

        /// <summary>
        /// Creates an object of TRootType and then recursively creates and hooks up its dependencies
        /// </summary>
        /// <typeparam name="TRootType">Typically, the root node of a tree of objects </typeparam>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take the previous saved instance 
        ///     of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///   Pass null if no additional assemblies are required. 
        ///   The assembly in which the call to this method is made is included by default
        ///   irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (TRootType rootBean, InjectionState injectionState)
          CreateAndInjectDependencies<TRootType>(InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            (object rootObject, InjectionState newInjectionState)
                = CreateAndInjectDependencies(typeof(TRootType), injectionState
                ,assemblies, rootBeanSpec);
            return ((TRootType) rootObject, newInjectionState);
        }

        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootType">Typically, the root node of a tree of objects to be created by this call </param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///   Pass null if no additional assemblies are required. 
        ///   The assembly in which the call to this method is made is included by default
        ///   irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState)
          CreateAndInjectDependencies(Type rootType, InjectionState injectionState = null
          , Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            rootBeanSpec = rootBeanSpec ?? new RootBeanSpec();
            (string rootBeanName, string rootConstructorName, BeanScope scope) = rootBeanSpec;
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            CheckArgument(rootType);
            CheckInjectionStateArgument(injectionState);

            object rootObject;
            InjectionState newInjectionState = CloneOrCreateInjectionState(rootType, injectionState
              , assemblies ?? new Assembly[0]);
            (rootObject, newInjectionState) 
                = CreateAndInjectDependenciesExCommon(
                rootType, newInjectionState, rootBeanName, rootConstructorName, scope);
            return (rootObject, newInjectionState);
        }

        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootTypeName">Typically, the name of a class for which an object
        ///     will be instantiated by this call.
        ///     Fully specified name including namespace</param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///   Pass null if no additional assemblies are required. 
        ///   The assembly in which the call to this method is made is included by default
        ///   irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(string rootTypeName, InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null)
        {
            rootBeanSpec = rootBeanSpec ?? new RootBeanSpec();
            (string rootBeanName, string rootConstructorName, BeanScope scope) = rootBeanSpec;
            CheckArgument(rootTypeName);
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            CheckInjectionStateArgument(injectionState);

            InjectionState newInjectionState = CloneOrCreateInjectionState(this.GetType(),injectionState
              , assemblies ?? new Assembly[0]);
            (Type rootType, string beanName) = newInjectionState.TypeMap.Keys.FirstOrDefault(k
              => AreTypeNamesEqualish(k.beanType.FullName, rootTypeName));
            if (rootType == null)
            {
                string allAssemblyNames
                    = ((dynamic)newInjectionState.Diagnostics.Groups[Constants.AssembliesInfo].Occurrences[0]).Assemblies;
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
        /// It may be appropriate to make one or more calls to create objects that will be needed in the tree
        /// before using a different call to create the full tree.
        /// Currently this method will attempt to recursively inject dependencies for rootObject and
        /// this may not be the desired behaviour.  This is a shortcoming and will be addressed
        /// in a future version.
        /// </summary>
        /// <param name="rootObject">some instantiated object which the library user needs
        ///     to attach to the object tree</param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///   Pass null if no additional assemblies are required. 
        ///   The assembly in which the call to this method is made is included by default
        ///   irrespective of the argument passed here</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState) 
          CreateAndInjectDependencies(object rootObject, InjectionState injectionState = null, Assembly[] assemblies = null)
        {
            CheckArgument(rootObject);
            CheckInjectionStateArgument(injectionState);

            InjectionState newInjectionState = CloneOrCreateInjectionState(rootObject.GetType(), injectionState
              , assemblies ?? new Assembly[0]);
            newInjectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(this.GetType(), Constants.DefaultBeanName
              ,Constants.DefaultConstructorName)] = this;
            ObjectTree tree = new ObjectTree();
            newInjectionState = tree.CreateAndInjectDependencies(rootObject, newInjectionState);
            return (rootObject, newInjectionState);
        }
        /// <summary>
        /// This version of the injection method allows the library user to instantiate an object
        /// using "new" or by whatever other means and have it injected into the object tree.
        /// It may be appropriate to make one or more calls to create objects that will be needed in the tree
        /// before using a different call to create the full tree.
        /// Currently this method will attempt to recursively inject dependencies for rootObject and
        /// this may not be the desired behaviour.  This is a shortcoming and will be addressed
        /// in a future version.
        /// </summary>
        /// <param name="rootObject">some instantiated object which the library user needs
        ///     to attach to the object tree</param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///   Pass null if no additional assemblies are required. 
        ///   The assembly in which the call to this method is made is included by default
        ///   irrespective of the argument passed here</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public (object rootBean, InjectionState injectionState) 
          CreateAndInjectDependenciesNewish(object rootObject, InjectionState injectionState = null, Assembly[] assemblies = null)
        {
            CheckArgument(rootObject);
            CheckInjectionStateArgument(injectionState);

            InjectionState newInjectionState = CloneOrCreateInjectionState(rootObject.GetType(), injectionState
              , assemblies ?? new Assembly[0]);
            string beanName = Guid.NewGuid().ToString().ToLower();
            newInjectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(this.GetType()
              ,Constants.DefaultBeanName, Constants.DefaultConstructorName)] = this;
            newInjectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(rootObject.GetType()
              ,beanName, Constants.DefaultConstructorName)] = rootObject;
//            newInjectionState = AddRootObjectDetails(newInjectionState, (rootObject.GetType(), beanName));
            ObjectTree tree = new ObjectTree();
            (_, newInjectionState) = new ObjectTree().CreateAndInjectDependencies(rootObject.GetType(), newInjectionState
            , beanName, Constants.DefaultConstructorName, BeanScope.Singleton);
            return (rootObject, newInjectionState);
        }
        /// <summary>
        /// <see cref="CreateAndInjectDependencies(string,PureDI.InjectionState,System.Reflection.Assembly[],PureDI.RootBeanSpec)"/>
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
            IReadOnlyDictionary<(Type beanType, string beanName), Type> typeMap;
            IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar;
            Assembly[] assemblies;
            (_, typeMap, mapObjectsCreatedSoFar, assemblies) = injectionState;
            injectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(this.GetType()
              ,Constants.DefaultBeanName
              ,Constants.DefaultConstructorName)] = this;
                // factories and possibly other beans may need access to the PDependencyInjector itself
                // so we include it as a bean by default
            if (mapObjectsCreatedSoFar.ContainsKey(new InstantiatedBeanId(rootType, rootBeanName
              ,Constants.DefaultConstructorName)))
            {
                return (mapObjectsCreatedSoFar[
                  new InstantiatedBeanId(rootType, rootBeanName, Constants.DefaultConstructorName)]
                    , new InjectionState(
                        injectionState.Diagnostics
                        , typeMap
                        , mapObjectsCreatedSoFar
                        , assemblies, injectionState.CreationContext));
            }
            object rootObject;
            (rootObject, injectionState) = new ObjectTree().CreateAndInjectDependencies(
              rootType, injectionState, rootBeanName.ToLower(), rootConstructorName.ToLower(), scope);
            if (rootObject == null && injectionState.Diagnostics.HasWarnings)
            {
                throw new DIException("Failed to create object tree - see diagnostics for details", injectionState.Diagnostics);
            }
            Assert(rootType.IsAssignableFrom(rootObject.GetType()));
            return (rootObject, injectionState);
        }
        private InjectionState CloneOrCreateInjectionState(Type rootType, InjectionState injectionState, Assembly[] explicitAssemblies)
        {
            InjectionState newInjectionState;
            IReadOnlyDictionary<(Type beanType, string beanName), Type> typeMap;
            Diagnostics diagnostics;
            if (injectionState == null || injectionState.IsEmpty())
            {
                var mapObjectsCreatedSoFar =
                    new Dictionary<InstantiatedBeanId, object>();
                (typeMap, diagnostics) = CreateTypeMap(rootType, explicitAssemblies);
                newInjectionState = new InjectionState(diagnostics, typeMap, mapObjectsCreatedSoFar
                  , explicitAssemblies, null);
            }
            else
            {
                Assembly[] assemblies = explicitAssemblies.Union(injectionState.Assemblies).ToArray();
                (typeMap, diagnostics) = CreateTypeMap(rootType
                  , assemblies);
                newInjectionState = new InjectionState(diagnostics, typeMap
                  , injectionState.MapObjectsCreatedSoFar.ToDictionary(kv => kv.Key, kv => kv.Value)
                  ,assemblies, injectionState.CreationContext);                
            }
            return newInjectionState;
        }
        
        private (IReadOnlyDictionary<(Type beanType, string beanName), Type>, Diagnostics diagnostics)
          CreateTypeMap(Type rootType, Assembly[] assemblies)
        {
            Diagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
            IReadOnlyDictionary<(Type beanType, string beanName), Type> typeMap = null;

            IReadOnlyList<Assembly> allAssemblies 
              = assemblies
              .Union(new[] { rootType.Assembly }.Where(a => !_ignoreRootTypeAssembly))
              .Union(new[] { this.GetType().Assembly })
              .ToList();
            new BeanValidator().ValidateAssemblies(allAssemblies, diagnostics);
            if (typeMap == null)
            {
                typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(allAssemblies
                    , ref diagnostics, _profileSet, os);
                LogAssemblies(diagnostics, allAssemblies);
                LogProfiles(diagnostics, _profileSet);
                LogTypeMap(diagnostics, typeMap);
            }
            return (typeMap, diagnostics);
        }

        private void LogTypeMap(Diagnostics diagnostics
          , IReadOnlyDictionary<(Type beanType, string beanName), Type> types)
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
          , IReadOnlyList<Assembly> assemblies)
        {
            Diagnostics.Group group = diagnostics.Groups[Constants.AssembliesInfo];
            dynamic diag = group.CreateDiagnostic();
            diag.Assemblies = string.Join(",", assemblies.Select(a => a.GetName().Name));
            diagnostics.Groups[Constants.AssembliesInfo].Occurrences.Add(diag);
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
        private void CheckNoBlankProfiles(string[] o)
        {
            if (o == null)
            {
                return;
            }
            if (o.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException();
            }
        }
        private void CheckInjectionStateArgument(InjectionState injectionState)
        {
            if (injectionState == null)
            {
                int previousValue = Interlocked.CompareExchange(ref _multipleCallGuard, 1, 0);
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
    /// The BeanScope enum is used in
    /// conjunction with bean references to determine
    /// how multiple references to a particular bean will be
    /// dealt with.  Where the scope is Singleton (which is
    /// the default) then all references with that
    /// scope will point to the same object.  Any
    /// reference with a scope of Prototype will point
    /// to separate objects.
    /// </summary>
    /// <remarks>
    /// An example of where this might be used is where processing of a web request
    /// requires a different user record to be instantiated
    /// for each request.
    /// </remarks>
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
        public static string ListContents(this IReadOnlyList<Assembly> assemblies, string separator = ", ")
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
            return (BeanReferenceBaseAttribute)type.GetCustomAttributes(
                ).FirstOrDefault(ca => ca is BeanReferenceBaseAttribute);
        }
        public static BeanReferenceBaseAttribute GetBeanReferenceAttribute(this ParameterInfo type)
        {
            return (BeanReferenceBaseAttribute)type.GetCustomAttributes(
                ).FirstOrDefault(ca => ca is BeanReferenceBaseAttribute);
        }
    }
}
