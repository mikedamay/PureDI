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
    // DONE An optional name should be passed to PDependencyInjector.GetOrCreateDependencyTree
    // DONE address static fields and beans.  Beans are invalid
    // DONE readonly fields
    // DONE document / investigate other classes derived from ValueType - nothing much to say
    // DONE inherited attributes
    //
    // DONE change wording of no-arg constructor diagnostic to include constructor based injections
    // TODO documentation:
    // TODO API reference
    // TODO set up remarks and notes as top level section headings in each topic
    // TODO FAQ / Workarounds
    // TODO embolden key text
    // TODO how to handle injection state passed to an injector with a different
    // TODO set of profiles
    // DONE upload docs to mikedamay.co.uk/PureDI
    // N/A document the fact that member type is based on the type's GetIOCCName() attribute - IOCCName == FullName
    // N/A and that generics have the for classname`1[TypeParam]
    // TODO explain how inheritance, factory with bean name, a separate base factory with Ignore
    // TODO are combined to support inheritance.  Execute must be virtual.
    // TODO in gotchas we claim that a dangling bean reference will be assigned
    // TODO its default value - is that true?
    // DONE document that names apply to factories not the target reference bean.
    // DONE document lack of thread safety
    // DONE document the point that injected members are not available in the constructor
    // DONE document the variable parameters issue or possibly implement a solution
    // DONE handle situation / document where sometimes in same program you want
    // DONE alternative implementations and sometimes you want parallel
    // TODO implementations a la IPropertyMap
    // TODO document that it is not possible to have OS.Any along with OS.Specific
    // DONE document bean, bean definition and other technical terms
    // TODO developer guide: policy on diagnostics and variation for constructors
    // TODO assert that attribute parameters are non-null and of the correct type.
    // TODO document that we can't handle with same type from multiple assemblies using aliases - I think this will defeat the IOCC
    // TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    // TODO test global:: and document that it won't work for root type passed as string
    // TODO spell check documentation
    // N/A sort out problem with angle brackets e.g. IEnumerable<T> in Limitations & Gotchas
    // DONE we need to say or do something about processing in constructors
    // DONE before the container builder has finished its business.
    // TODO bean names, constructor names and profiles are case insensitive
    // N/A Even if beans are referenced only by factories they still need names to 
    // N/A distinguish multiple implementations of the same interface.
    // N/A Of course classes referenced by factories don't have to be beans.
    // N/A factory beans are typically (but not necessarily) created as prototypes so
    // N/A if there is another non-factory based injection of the bean it will be
    // N/A a different instance
    // N/A add version to documentation heading
    // 
    // DONE handle DocumentParser scenario where two beans are required with varying parameters.
    // N/A constructor name needs to be included in the cached tree
    // N/A document use of profiles with factories
    // DONE profile should take the best fit of implementation
    // TODO Implementation:
    // TODO ExcludeSimpleIOCCContainer name change in main code
    // TODO ExcludeSimpleIOCCContainer name change in SandDoc
    // TODO Move UserGuide.xml from PureDI to PureDiDocumentor
    // DONE test with multiple OSs
    // TODO can we handle an object or type from some assembly as root which is not
    // TODO a scanned assembly
    // DONE tidy up DiagnosticSchema to remove references to user guide.
    // N/A change names from PDependencyInjector to SimpleDependencyInjector
    // N/A remove nocache headers from documentation
    // DONE can we handle bean references in a base class?  Tests required
    // DONE do we need reconsider abstract base classes?  I think we're ok we pick up inherited members
    // DONE make typemap and mapCreatedSoFar parameters to CreateAndinjectDependencies
    // DONE we need an overload that takes a type
    // DONE add exception handling to other entry points.
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
    // N/A Apply the PDependencyInjector to the Calculation Server and Maven docs - not very useful
    // DONE Release Build
    // DONE remove 2-way enumerator
    // N/A improve performance of IOCCObjectTree.CreateObjectTree with respect to dictionary handling - no prob with perf
    // DONE Perf
    // DONE Test with nullables
    // DONE make PDependencyInjector instance a bean by default.
    // DONE generics for factories
    // DONE decide if scope on factory reference refers to factory or reference to be created
    // DONE it makes no sense for the created object as the scope is under the control of the factory
    // DONE does it make any sense in the case of the factory - it does, a little
    // DONE allow root type a prototype
    // DONE test with no namespace
    // DONE change name to PDependencyInjector from PDependencyInjector
    // N/A optimised build - there doesn't seem to be any optimisation tuning
    // DONE guard against static constructors
    // DONE automatically add PDependencyInjector to the list of assemblies
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
    // TODO Later: add references to help to diagnostics
    // TODO Later: rudimentary developer guide
    // TODO Later: look at MEF implementations - heard on dnr 8-8-17
    // TODO Later: testing in untrusted environments
    // TODO Later: ninject
    // TODO Later: spring
    // TODO Later: ASP.NET
    // TODO Later: Mass Test - 2 days
    // TODO Later: red team: deep hierarchies
    // TODO Later: red team: mix new and CreateAndInject...
    // TODO Later: red team: self registering classes - that are also beans
    // TODO Later: PDependencyInjector or InjectionState should expose active
    // TODO Later: profiles, particularly for use by factories.
    // TODO Later: allow user to pass a flag to the injector constructor to treate warnings as errors
    // TODO Later: add an interface for PDependencyInjector
    // TODO docs: DI-OddsAndEnds examples for object cycles
    // TODO docs: DI-Assemblies example of including a reference to PDependencyInjector
    // TODO docs: in a factory bean
    // TODO docs: example of assembly exclusion
    // TODO docs: connect up summary and details in DesignRationale

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
