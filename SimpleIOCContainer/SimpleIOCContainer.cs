using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using com.TheDisappointedProgrammer.IOCC.Common;
using com.TheDisappointedProgrammer.IOCC.Tree;
using static com.TheDisappointedProgrammer.IOCC.Common.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    // DONE constructor paramter injection
    // DONE prototypes
    // DONE warning when a field or property has already been initialised
    // DONE guard against circular references - Done
    // DONE handle structs
    // DONE handle properties
    // DONE object factories
    // DONE handle multiple assemblies - Done
    // DONE references held in tuples
    // DONE references held in embedded structs 
    // DONE references held as objects
    // DONE references to arrays
    // DONE use fully qualified type names in comparisons
    // N/A use immutable collections - they don't do much for us at any level
    // DONE detect duplicate type, name, profile, os combos (ensure any are compared to specific os and profile)
    // DONE handle or document generic classes
    // DONE handle dynamic types
    // DONE change "dependency" to "bean"
    // DONE check arguments' validity for externally facing methods
    // DONE unit test to validate XML - Done
    // N/A run code analysis - doesn't seem to do anything
    // DONE make Diagnostics constructor private
    // DONE improve names of queries assigned from complex linq structures
    // N/A use immutable collections - I don't think they do anything for us at any level
    // DONE license
    // DONE An optional name should be passed to SimpleIOCContainer.GetOrCreateDependencyTree
    // DONE address static fields and beans.  Beans are invalid
    // DONE readonly fields
    // DONE document / investigate other classes derived from ValueType - nothing much to say
    // DONE inherited attributes
    //
    // DONE change wording of no-arg constructor diagnostic to include constructor based injections
    // TODO Research:
    // TODO look at MEF implementations - heard on dnr 8-8-17
    // TODO testing in untrusted environments
    // TODO ninject
    // TODO spring
    // TODO ASP.NET
    //
    // TODO documentation:
    // TODO API reference
    // TODO rudimentary developer guide
    // N/A document the fact that member type is based on the type's GetIOCCName() attribute - IOCCName == FullName
    // N/A and that generics have the for classname`1[TypeParam]
    // TODO explain how inheritance, factory with bean name, a separate base factory with IOCCIgnore
    // TODO are combined to support inheritance.  Execute must be virtual.
    // DONE document that names apply to factories not the target reference bean.
    // DONE document lack of thread safety
    // DONE document the point that injected members are not available in the constructor
    // DONE document the variable parameters issue or possibly implement a solution
    // DONE handle situation / document where sometimes in same program you want
    // DONE alternative implementations and sometimes you want parallel
    // TODO implementations a la IPropertyMap
    // TODO document that it is not possible to have OS.Any along with OS.Specific
    // TODO document bean, bean definition and other technical terms
    // TODO developer guide: policy on diagnostics and variation for constructors
    // TODO assert that attribute parameters are non-null and of the correct type.
    // TODO document that we can't handle with same type from multiple assemblies using aliases - I think this will defeat the IOCC
    // TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    // TODO test global:: and document that it won't work for root type passed as string
    // TODO spell check documentation
    // TODO sort out problem with angle brackets e.g. IEnumerable<T> in Limitations & Gotchas
    // TODO factory beans are typically (but not necessarily) created as prototypes so
    // TODO if there is another non-factory based injecttion of the bean it will be
    // TODO a different instance
    // TODO add version to documentation heading
    // 
    // DONE handle DocumentParser scenario where two beans are required with varying parameters.
    // N/A constructor name needs to be included in the cached tree
    // N/A document use of profiles with factories
    // DONE profile should take the best fit of implementation
    // TODO Implementation:
    // TODO Mass Test - 2 days
    // TODO test with multiple OSs
    // TODO remove nocache headers from documentation
    // TODO can we handle bean references in a base class?  Tests required
    // TODO do we need reconsider abstract base classes?  I think we're ok we pick up inherited members
    // TODO make typemap and mapCreatedSoFar parameters to CreateAndinjectDependencies
    // TODO we need to say or do something about processing in constructors
    // TODO before the container builder has finished its business.
    // TODO red team: deep hierarchies
    // TODO red team: mix new and CreateAndInject...
    // TODO red team: self registering classes - that are also beans
    // DONE heading for diagnostic output e.g. Diagnostic Information:
    // DONE add logging for inspection of assemblies and disposition of types - .5 days
    // N/A add constructor name to map...CreatedSoFar... - i day
    // DONE test case-sensitivity
    // N/A nuget - i day
    // DONE generate github docs
    // DONE build project from a different path
    // N/A change HasWarnings to HasDiagnostics
    // DONE prevent user from passing null or empty string to container constructor
    // DONE check that rootObject instantiated directly can be found in the tree (this vs. rootObject in assemblyNames)
    // DONE make assembly lists immutable
    // N/A suppress code analysis messages - doesn't seem to work
    // DONE move the majority of unit tests to separate assemblies
    // DONE test generics with multiple parameters
    // DONE test generics with nested parameters
    // DONE ensure there is a test that uses an object multiple times in the tree. - ShouldCreateASingleInstanceForMultipleReferences
    // DONE ensure where interface->base class->derived class occurs there is no problem with duplication of beans
    // DONE make sure that root failure when passing type string is handled via diagnostics and that
    // DONE the explanation is expanded to include that.
    // N/A make our own constructor to handle readonly properties - not a good idea
    // N/A change Docs folder to resources folder
    // N/A Apply the SimpleIOCContainer to the Calculation Server and Maven docs - not very useful
    // DONE Release Build
    // DONE remove 2-way enumerator
    // N/A improve performance of IOCCObjectTree.CreateObjectTree with respect to dictionary handling - no prob with perf
    // DONE Perf
    // DONE Test with nullables
    // DONE make SimpleIOCContainer instance a bean by default.
    // DONE generics for factories
    // DONE decide if scope on factory reference refers to factory or reference to be created
    // DONE it makes no sense for the created object as the scope is under the control of the factory
    // DONE does it make any sense in the case of the factory - it does, a little
    // DONE allow root type a prototype
    // DONE test with no namespace
    // DONE change name to SimpleIOCContainer from SimpleIOCContainer
    // N/A optimised build - there doesn't seem to be any optimisation tuning
    // DONE guard against static constructors
    // DONE automatically add SimpleIOCContainer to the list of assemblies
    // DONE write out the diagnostic schema from resource before validation test
    // DONE document that IFactory is ignored as an interface.  Workaround to create intermediate
    // DONE this is the first gotcha
    // DONE name and profile get mixed up - maybe a profile set will help - second gotcha
    // DONE warn of factories that do not have IFactory as interface
    // DONE warn of factories that do not have [Bean] attribute
    // DONE change FactoryParam to an object
    // DONE make bean names and profiles case insensitive
    // DONE test with private classes
    // DONE test with attributes as beans - nothing special
    // DONE test passing an interface as root type
    // DONE test with multiple attributes
    // TODO Later: implement constructor parameters
    // TODO Later: allow arbitrary objects to be attached to the tree.
    // TODO Later: built-in factories for environement variables, command line arguments, config files
    // TODO Later: seed the tree with a specific implementation
    // TODO Later: syntax colorisation in documentation
    // TODO Later: Code Analysis
    // TODO Later: deal with exceptions on nested calls to CreateAndInject...()
    // TODO Later: handle extern alias situations
    // TODO Later: move documentation site to TheDisappointedProgrammer.com
    // TODO Later: diagnostics combination functionality
    // TODO Later: Make constructors so that we can inject readonly properties - maybe - probably not
    // TODO Later: handle whether external interfaces are included or excluded from typeMap
    // TODO Later: add tables to markdown
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// constraints:
    ///     6) Even if beans are referenced only by factories they still names to 
    ///        distinguish multiple implementations of the same interfacr.
    ///        Of course classes referenced by factories don't have to be beans.
    ///     7) bean names, constructor names and profiles are case insensitive
    /// </remarks>
    [Bean]
    public partial class SimpleIOCContainer
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
            /// Any MAC verion supported by dotnetstandard 2.0
            /// </summary>
            MacOS
        }
        private readonly OS os = new StdOSDetector().DetectOS();
        internal static SimpleIOCContainer Instance { get; } = new SimpleIOCContainer();
        internal const string DEFAULT_PROFILE_ARG = "";
        internal const string DEFAULT_BEAN_NAME = "";
        internal const string DEFAULT_CONSTRUCTOR_NAME = "";

        [Flags]
        public enum AssemblyExclusion { ExcludedNone = 0
          , ExcludeSimpleIOCCContainer = 1
          , ExcludeRootTypeAssembly = 2}
        private readonly AssemblyExclusion excludedAssemblies;
        private readonly ImmutableArray<Assembly> explicitAssemblies;
        private readonly ISet<string> profileSet;

        // the key in the objects created so far map comprises 2 types.  The first is the
        // intended concrete type that will be instantiated.  This works well for
        // non-generic types but for generics the concrete type, which is taken from the typeMap,
        // is a generic type definition.  The builder needs to lay its hands on the type argument
        // to substitute for the generic parameter.  The second type (beanReferenceType) which
        // has been taken from the member information of the declaring task provides the generic argument
        private IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
            new Dictionary<(Type, string), object>();

        private IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap;

        /// <summary>
        /// this routine is called to specify the assemblies to be scanned
        /// for beans.  Any bean to be injected must be defined in one
        /// of these assemblies and must be marked with the [Bean] attribute.
        /// </summary>
        /// <remarks>
        /// The assembly containing SimpleIOCCBean class itself is always included
        /// by default.  It does not need to be specified.  The purpose
        /// of the inclusion is to allow callers to include the SimpleIOCContainer
        /// bean itself in factories.  The assumbly is included to make this intuitive.
        /// </remarks>
        /// <example>SetAssemblies( true, "MyApp", "MyLib")</example>
        /// <param name="excludeRootAssembly">By default the assembly containing the type
        /// passed to CreateAndInjectDependencies() is included automatically.
        /// Pass true here to ensure it is not scanned for beans.
        /// Note that if you include the root assembly in the list
        /// of assemblies then the excludeRootAssembly flag is ignored.
        /// Note that if a string containing the root type is passed
        /// to CreateAndInjectDependencies() then the system behaves as if
        /// the flag was set to true as there is no easy way for the container
        /// to know the assembly from which it was called.</param>
        public SimpleIOCContainer( string[] Profiles = null
          , Assembly[] Assemblies = null
          , AssemblyExclusion ExcludeAssemblies =  AssemblyExclusion.ExcludedNone)
        {
            CheckProfilesArgument(Profiles);
            excludedAssemblies = ExcludeAssemblies;
            explicitAssemblies = (Assemblies != null 
              ? ImmutableArray.Create<Assembly>(Assemblies) 
              : ImmutableArray<Assembly>.Empty).ToImmutableArray();
            profileSet = new HashSet<string>( Profiles 
              ?? new string[0], new CaseInsensitiveEqualityComparer());
        }

        /// <param name="injectionState"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        ///     top of the tree - as instantiated by rootType
        ///     It does not affect the rest of the tree.  The other nodes on the tree will
        ///     honour the Scope property of [IOCCBeanReference]</param>
        public (TRootType rootBean, InjectionState injectionState) 
          CreateAndInjectDependencies<TRootType>(InjectionState injectionState = null, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            try
            {
                IOCCDiagnostics diagnostics;
                CheckArgument(rootBeanName);
                CheckArgument(rootConstructorName);
                ISet<string> profileSet;
                (typeMap, diagnostics, profileSet) = CreateTypeMap(typeof(TRootType));
                var rootObject = (TRootType)CreateAndInjectDependenciesExCommon(typeof(TRootType), diagnostics, profileSet, rootBeanName, rootConstructorName, scope);
                return (rootObject
                    , new InjectionState(
                        diagnostics
                        , new WouldBeImmutableDictionary<(Type beanType, string beanName), Type>()
                        , new Dictionary<(Type, string), object>()
                    ));

            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case IOCCException iex:
                        throw;
                    case IOCCInternalException iiex:
                        throw;
                    case ArgumentNullException anx:
                        throw;
                    default:
                        // TODO we need to do something or say something about diagnostics
                        throw new IOCCException("Injection dependency failed.  Please the constructors of beans to ensure they are not accessing other beans prematurely"
                          ,new DiagnosticBuilder().Diagnostics);

                }
            }        
        }
        // TODO complete the documentation item 3 below if and when factory types are implemented
        // TODO handle situation where there is no console window
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <param name="beanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        ///     top of the tree - as instantiated by rootType
        ///     It does not affect the rest of the tree.  The other nodes on the tree will
        ///     honour the Scope property of [IOCCBeanReference]</param>
        /// <returns>an object of root type</returns>
#if false
        public TRootType CreateAndInjectDependencies<TRootType>(string beanName = DEFAULT_BEAN_NAME
          , string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(beanName);
            CheckArgument(rootConstructorName);
            ISet<string> profileSet;
            IOCCDiagnostics diagnostics = null;
            TRootType rootObject = default;
            try
            {
                (typeMap, diagnostics, profileSet) = CreateTypeMap(typeof(TRootType));
                rootObject = (TRootType)CreateAndInjectDependenciesExCommon(typeof(TRootType), diagnostics, profileSet, beanName, rootConstructorName, scope);
            }
            finally
            {
                if (diagnostics != null)
                {                  
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debug.WriteLine(diagnostics);
                    }
                    else
                    {
                        Console.WriteLine(diagnostics);    
                    }                 
                }
            }
            return rootObject;
        }
#endif
        /// <param name="rootTypeName">provided by caller - <see cref="AreTypeNamesEqualish"/></param>
        /// <param name="injectionState"></param>
        /// <param name="rootBeanName">an SimpleIOCContainer type spec in the form "MyNameSpace.MyClass"
        ///     or "MyNameSpace.MyClass&lt;MyActualParam &gt;" or
        ///     where inner classes are involved "MyNameSpace.MyClass+MyInnerClass"</param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        ///     top of the tree - as instantiated by rootTypeName
        ///     It does not affect the rest of the tree.  The other nodes on the tree will
        ///     honour the Scope property of [IOCCBeanReference]</param>
        /// <returns>the root of the object tree with all dependencies instantiated</returns>
        public (object rootBean, InjectionState injectionState) CreateAndInjectDependenciesWithString(string rootTypeName, InjectionState injectionState = null, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(rootTypeName);
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            IOCCDiagnostics diagnostics;
            ISet<string> profileSet;
            (typeMap, diagnostics, profileSet) = CreateTypeMap(this.GetType());
            (Type rootType, string beanName) = typeMap.Keys.FirstOrDefault(k 
              => AreTypeNamesEqualish(k.beanType.FullName, rootTypeName));
            if (rootType == null)
            {
                string allAssemblyNames
                    = ((dynamic) diagnostics.Groups[Constants.ASSEMBLIES_INFO].Occurrences[0]).Assemblies;
                IOCCDiagnostics.Group group = diagnostics.Groups["MissingRootBean"];
                dynamic diag = group.CreateDiagnostic();
                diag.BeanType = rootTypeName;
                diag.BeanName = rootBeanName;
                group.Add(diag);
                throw new IOCCException($"Unable to find a type in assembly {allAssemblyNames} for {rootTypeName}{Environment.NewLine}Remember to include the namespace", diagnostics);
            }
            object rootObject = CreateAndInjectDependenciesExCommon(rootType
                ,diagnostics, profileSet, rootBeanName
                ,rootConstructorName, scope);
            return (rootObject
                , new InjectionState(
                    diagnostics
                    , new WouldBeImmutableDictionary<(Type beanType, string beanName), Type>()
                    , new Dictionary<(Type, string), object>()
                ));
        }

        public (object rootBean, InjectionState injectionState) CreateAndInjectDependenciesWithObject(object rootObject, InjectionState injectionState = null)
        {
            ISet<string> profileSet;
            IOCCDiagnostics diagnostics;
            (typeMap, diagnostics, profileSet) = CreateTypeMap(rootObject.GetType());
            string profileSetKey = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            if ((excludedAssemblies & AssemblyExclusion.ExcludeSimpleIOCCContainer) == 0)
            {
                mapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
            }
            if ((excludedAssemblies & AssemblyExclusion.ExcludeRootTypeAssembly) == 0)
            {
                mapObjectsCreatedSoFar[(rootObject.GetType(), DEFAULT_BEAN_NAME)] = rootObject;
            }
            string profile = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            ObjectTree tree = new ObjectTree(profile, typeMap);
            tree.CreateAndInjectDependencies(rootObject, diagnostics
                , mapObjectsCreatedSoFar);

            return (rootObject
                , new InjectionState(
                    diagnostics
                    , new WouldBeImmutableDictionary<(Type beanType, string beanName), Type>()
                    , new Dictionary<(Type, string), object>()
                ));
        }
        /// <summary>
        /// <see cref="CreateAndInjectDependenciesWithString"/>
        /// this overload does not print out the diagnostics
        /// </summary>
        /// <param name="rootType"></param>
        /// <param name="diagnostics">This overload exposes the diagnostics object to the caller</param>
        /// <param name="profile"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope"></param>
        private object CreateAndInjectDependenciesExCommon(Type rootType
          , IOCCDiagnostics diagnostics, ISet<string> profileSet, string rootBeanName
          , string rootConstructorName, BeanScope scope)
        {
            string profileSetKey = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            if ((excludedAssemblies & AssemblyExclusion.ExcludeSimpleIOCCContainer) == 0)
            {
                mapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
                // factories and possibly other beans may need access to the SimpleIOCContainer itself
                // so we include it as a bean by default
            }
            if (mapObjectsCreatedSoFar.ContainsKey((rootType, rootBeanName)))
            {
                return mapObjectsCreatedSoFar[(rootType, rootBeanName)];
            }
            ObjectTree tree = new ObjectTree(profileSetKey, typeMap);
            var rootObject = tree.CreateAndInjectDependencies(
              rootType, diagnostics, rootBeanName.ToLower(), rootConstructorName.ToLower()
              , scope, mapObjectsCreatedSoFar);
            if (rootObject == null && diagnostics.HasWarnings)
            {
                throw new IOCCException("Failed to create object tree - see diagnostics for details", diagnostics);
            }
            Assert(rootType.IsAssignableFrom(rootObject.GetType()));
            return rootObject;
        }

        private (IWouldBeImmutableDictionary<(Type beanType, string beanName)
          , Type>, IOCCDiagnostics diagnostics, ISet<string> profileSet)
          CreateTypeMap(Type rootType)
        {
            IOCCDiagnostics diagnostics = new DiagnosticBuilder().Diagnostics;

            // make sure that the IOC Container itself is available as a bean
            // particularly to factories
            var builder = ImmutableList.CreateBuilder<Assembly>();
                builder.AddRange(explicitAssemblies
                    .Union(new[] {rootType.Assembly}.Where( a =>
                        (excludedAssemblies & AssemblyExclusion.ExcludeRootTypeAssembly) == 0))
                        .Union(new[] {this.GetType().Assembly}.Where( a =>
                        (excludedAssemblies & AssemblyExclusion.ExcludeSimpleIOCCContainer) == 0)));
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
            return (typeMap, diagnostics, profileSet);
        }

        private void LogTypeMap(IOCCDiagnostics diagnostics
          , IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> types)
        {
            IEnumerable<(Type, string, Type)> typesQuery;
            if (types.Count == 0)
            {
                typesQuery = new(Type, string, Type)[] {(null, "none", null)};
            }
            else
            {
                typesQuery = types.Select(kv => (kv.Key.beanType, kv.Key.beanName, kv.Value));
            }
            IOCCDiagnostics.Group group = diagnostics.Groups["TypesInfo"];
            dynamic diag = group.CreateDiagnostic();
            foreach (var ti in typesQuery)
            {
                diag.ReferenceType = ti.Item1?.Name ?? "none";
                diag.ReferenceType = ti.Item2 ?? "none";
                diag.ReferenceType = ti.Item3?.Name ?? "none";
            }
            diagnostics.Groups["TypesInfo"].Occurrences.Add(diag);
        }

        private void LogProfiles(IOCCDiagnostics diagnostics, ISet<string> profileSet)
        {
            IOCCDiagnostics.Group group = diagnostics.Groups["ProfilesInfo"];
            dynamic diag = group.CreateDiagnostic();
            diag.Profiles = string.Join(",", profileSet);
            diagnostics.Groups["ProfilesInfo"].Occurrences.Add(diag);
        }

        private void LogAssemblies(IOCCDiagnostics diagnostics
          , IImmutableList<Assembly> assemblies)
        {
            IOCCDiagnostics.Group group = diagnostics.Groups[Constants.ASSEMBLIES_INFO];
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
            ISet<string> assemblyNameSet = assemblyNames.ToHashSet( s => s);
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

    }   // SimpleIOCContainer

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

    public enum BeanScope { Singleton, Prototype}

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
