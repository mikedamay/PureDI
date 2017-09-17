using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using com.TheDisappointedProgrammer.IOCC.Tree;
using static com.TheDisappointedProgrammer.IOCC.Common;

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
    // TODO look at MEF implementations - heard on dnr 8-8-17
    // DONE document / investigate other classes derived from ValueType - nothing much to say
    // TODO testing in untrusted environments
    // DONE inherited attributes
    // TODO built-in factories for environement variables, command line arguments, config files
    // TODO attach existing object to object tree
    //
    // TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    // TODO test case-sensitivity
    // TODO change wording of no-arg constructor diagnostic to include constructor based injections
    // TODO document the fact that member type is based on the type's GetIOCCName() attribute
    // TODO and that generics have the for classname`1[TypeParam]
    // TODO explain how inheritance, factory with bean name, a separate base factory with IOCCIgnore
    // TODO are combined to support inheritance.  Execute must be virtual.
    // TODO document that names apply to factories not the target reference bean.
    // TODO document lack of thread safety
    // TODO document the point that injected members are not available in the constructor
    // TODO handle DocumentParser scenario where two beans are required with varying parameters.
    // TODO constructor name needs to be included in the cached tree
    // TODO document use of profiles with factories
    // TODO deal with exceptions on nested calls to CreateAndInject...()
    // N/A suppress code analysis messages - doesn't seem to work
    // DONE move the majority of unit tests to separate assemblies
    // DONE test generics with multiple parameters
    // DONE test generics with nested parameters
    // DONE ensure there is a test that uses an object multiple times in the tree. - ShouldCreateASingleInstanceForMultipleReferences
    // DONE ensure where interface->base class->derived class occurs there is no problem with duplication of beans
    // DONE make sure that root failure when passing type string is handled via diagnostics and that
    // DONE the explanation is expanded to include that.
    // N/A make our own constructor to handle readonly properties - not a good idea
    // TODO change HasWarnings to HasDiagnostics
    // TODO change Docs folder to resources folder
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
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// constraints:
    ///     1) the object tree (i.e. the program's static model) is required to be static.
    ///     if objects are added to the tree through code at run-time this will not be 
    ///     reflected in the IOC container.
    ///     2) The root class has to be visible to the caller of CreateAndInjectDependencies.
    ///     2a) The root of the tree cannot be specified using reflection.  I'll probably regret that.
    ///     3) static classes and members are not handled.
    ///     4) If a member is incorrectly marked as [IOCCBeanReference] then
    ///        it will be set to its default value even if it is an initialized member.
    ///     5) There is no way for the root bean to be a prototype
    ///     6) Even if beans are referenced only by factories they still names to 
    ///        distinguish multiple implementations of the same interfacr.
    ///        Of course classes referenced by factories don't have to be beans.
    ///     7) readonly properties cannot have their value set by injection.  A constructor
    ///        or field must be involved.  - we could make our own constructor
    ///     8) Note that beans are not available within constructors
    ///     7) bean names, constructor names and profiles are case insensitive
    ///     8) IFactory is ignored in resolving a bean reference - gotcha
    /// </remarks>
    [Bean]
    public class SimpleIOCContainer
    {
        public enum OS { Any, Linux, Windows, MacOS } OS os = new StdOSDetector().DetectOS();
        public static SimpleIOCContainer Instance { get; } = new SimpleIOCContainer();
        internal const string DEFAULT_PROFILE_ARG = "";
        internal const string DEFAULT_BEAN_NAME = "";
        internal const string DEFAULT_CONSTRUCTOR_NAME = "";

        private bool CreateAndInjectDependenciesCalled = false;
        private IList<string> assemblyNames = new List<string>();
        private readonly IDictionary<string, ObjectTreeContainer> mapObjectTreeContainers 
          = new Dictionary<string, ObjectTreeContainer>();

        private readonly ISet<string> profileSet;
        // the key in the objects created so far map comprises 2 types.  The first is the
        // intended concrete type that will be instantiated.  This works well for
        // non-generic types but for generics the concrete type, which is taken from the typeMap,
        // is a generic type definition.  The builder needs to lay its hands on the type argument
        // to substitute for the generic parameter.  The second type (beanReferenceType) which
        // has been taken from the member information of the declaring task provides the generic argument
        private IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
            new Dictionary<(Type, string), object>();

        private bool excludeRootAssembly;
        private IDictionary<(Type beanType, string beanName), Type> typeMap;

        public SimpleIOCContainer( params string[] profiles)
        {
            profileSet = new HashSet<string>(profiles.Length > 0 ? profiles : new string[0], new CaseInsensitiveEqualityComparer());
        }
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
        public void SetAssemblies(bool excludeRootAssembly, params string[] assemblyNames)
        {
            this.excludeRootAssembly = excludeRootAssembly;
            SetAssemblies(assemblyNames);
        }
        /// <example>SetAssemblies("MyApp", "MyLib")</example>
        public void SetAssemblies(params string[] assemblyNames)
        {
            CheckArgument(assemblyNames);
            if (CreateAndInjectDependenciesCalled)
            {
                throw new InvalidOperationException(
                  "SetAssemblies has been called after CreateAndInjectDependencies."
                  + "  This is not permitted.");
            }
            if (this.assemblyNames.Count > 0)
            {
                throw new IOCCException("SimpleIOCContainer.SetAssemblies() should be called only once"
                  ,null);
            }
            this.assemblyNames = assemblyNames.ToList();
        }

        /// <param name="diagnostics"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        ///     top of the tree - as instantiated by rootType
        ///     It does not affect the rest of the tree.  The other nodes on the tree will
        ///     honour the Scope property of [IOCCBeanReference]</param>
        public TRootType CreateAndInjectDependencies<TRootType>(out IOCCDiagnostics diagnostics, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            ISet<string> profileSet;
            (typeMap, diagnostics, profileSet) = CreateTypeMap(typeof(TRootType));
            return (TRootType)CreateAndInjectDependenciesExCommon(typeof(TRootType), diagnostics, profileSet, rootBeanName, rootConstructorName, scope);
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
        /// <returns>an ojbect of root type</returns>
        public TRootType CreateAndInjectDependencies<TRootType>(string beanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            //CheckArgument(profile);
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

        /// <param name="rootTypeName">provided by caller - <see cref="AreTypeNamesEqualish"/></param>
        /// <param name="diagnostics"></param>
        /// <param name="rootBeanName">an SimpleIOCContainer type spec in the form "MyNameSpace.MyClass"
        ///     or "MyNameSpace.MyClass&lt;MyActualParam&gt" or
        ///     where inner classes are involved "MyNameSpace.MyClass+MyInnerClass"</param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        ///     top of the tree - as instantiated by rootTypeName
        ///     It does not affect the rest of the tree.  The other nodes on the tree will
        ///     honour the Scope property of [IOCCBeanReference]</param>
        /// <returns></returns>
        public object CreateAndInjectDependencies(string rootTypeName, out IOCCDiagnostics diagnostics, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            CheckArgument(rootTypeName);
            //CheckArgument(profile);
            CheckArgument(rootBeanName);
            CheckArgument(rootConstructorName);
            IDictionary<(Type beanType, string beanName), Type> typeMap;
            ISet<string> profileSet;
            (typeMap, diagnostics, profileSet) = CreateTypeMap(this.GetType());
            (Type rootType, string beanName) = typeMap.Keys.FirstOrDefault(k 
              => AreTypeNamesEqualish(k.beanType.FullName, rootTypeName));
            if (rootType == null)
            {
                IOCCDiagnostics.Group group = diagnostics.Groups["MissingRootBean"];
                dynamic diag = group.CreateDiagnostic();
                diag.BeanType = rootTypeName;
                diag.BeanName = rootBeanName;
                group.Add(diag);
                throw new IOCCException($"Unable to find a type in assembly {assemblyNames.ListContents()} for {rootTypeName}{Environment.NewLine}Remember to include the namespace", diagnostics);
            }
            return CreateAndInjectDependenciesExCommon(rootType
                ,diagnostics, profileSet, rootBeanName
                ,rootConstructorName, scope);
        }

        public void CreateAndInjectDependencies(object rootObject, out IOCCDiagnostics diagnostics)
        {
            IDictionary<(Type beanType, string beanName), Type> typeMap;
            ISet<string> profileSet;
            (typeMap, diagnostics, profileSet) = CreateTypeMap(this.GetType());
            CreateAndInjectDependenciesCalled = true;
            ObjectTreeContainer container;
            string profileSetKey = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            if (mapObjectTreeContainers.ContainsKey(profileSetKey))
            {
                container = mapObjectTreeContainers[profileSetKey];
            }
            else
            {
                container = new ObjectTreeContainer(profileSetKey, typeMap);
            }
            mapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = rootObject;
            mapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
            //container.CreateAndInjectDependencies(rootObject, diagnostics, mapObjectsCreatedSoFar);
            string profile = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            ObjectTree tree = new ObjectTree(profile, typeMap);
            tree.CreateAndInjectDependencies(rootObject, diagnostics
                , mapObjectsCreatedSoFar);

        }
        /// <summary>
        /// <see cref="CreateAndInjectDependencies"/>
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
            CreateAndInjectDependenciesCalled = true;
            ObjectTreeContainer container;
            string profileSetKey = string.Join(" ", profileSet.OrderBy(p => p).ToList()).ToLower();
            if (mapObjectTreeContainers.ContainsKey(profileSetKey))
            {
                container = mapObjectTreeContainers[profileSetKey];
            }
            else
            {
                container = new ObjectTreeContainer(profileSetKey, typeMap);
            }
            mapObjectsCreatedSoFar[(this.GetType(), DEFAULT_BEAN_NAME)] = this;
                    // factories and possibly other beans may need access to the SimpleIOCContainer itself
                    // so we include it as a bean by default
            var rootObject = container.CreateAndInjectDependencies(rootType, diagnostics, rootBeanName.ToLower(), rootConstructorName.ToLower(), scope, mapObjectsCreatedSoFar);
            if (rootObject == null && diagnostics.HasWarnings)
            {
                throw new IOCCException("Failed to create object tree - see diagnostics for details", diagnostics);
            }
            Assert(rootType.IsAssignableFrom(rootObject.GetType()));
            return rootObject;
        }

        private (IDictionary<(Type beanType, string beanName), Type>, IOCCDiagnostics diagnostics, ISet<string> profileSet)
          CreateTypeMap(Type rootType)
        {
            IOCCDiagnostics diagnostics = new DiagnosticBuilder().Diagnostics;
            if (!excludeRootAssembly)
            {
                assemblyNames.Add(rootType.Assembly.GetName().Name);
            }
            assemblyNames.Add(this.GetType().Assembly.GetName().Name);
            // make sure that the IOC Container itself is available as a bean
            // particularly to factories
            IList<Assembly> assemblies = AssembleAssemblies(assemblyNames);
            new BeanValidator().ValidateAssemblies(assemblies, diagnostics);
            if (typeMap == null)
            {
                typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(assemblies
                    , ref diagnostics, profileSet, os);
            }
            return (typeMap, diagnostics, profileSet);
        }

        /// <summary>
        /// builds list of all the assemblies involved in the dependency tree
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

    }

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
// SimpleIOCContainer

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
