﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using static com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    // DONE constructor paramter injection
    // DONE prototypes
    // TODO warning when a field or property has already been initialised
    // DONE guard against circular references - Done
    // DONE handle structs
    // DONE handle properties
    // DONE object factories
    // DONE handle multiple assemblies - Done
    // TODO references held in tuples
    // TODO references held in embedded structs 
    // TODO references held as objects
    // TODO references to arrays
    // DONE use fully qualified type names in comparisons
    // TODO use immutable collections
    // TODO detect duplicate type, name, profile, os combos (ensure any are compared to specific os and profile)
    // DONE handle or document generic classes
    // TODO handle dynamic types
    // DONE change "dependency" to "bean"
    // TODO check arguments' validity for externally facing methods
    // DONE unit test to validate XML - Done
    // TODO run code analysis
    // DONE make Diagnostics constructor private
    // TODO improve names of queries assigned from complex linq structures
    // TODO use immutable collections
    // TODO license
    // TODO An optional name should be passed to SimpleIOCContainer.GetOrCreateDependencyTree
    // TODO address static fields and beans.  Beans are invalid or maybe not
    // DONE readonly fields
    // TODO look at MEF implementations - heard on dnr 8-8-17
    // TODO change text on ReadOnlyProperty to mention that this can be set by using the constructor
    // TODO suppress code analysis messages
    // TODO change wording of no-arg constructor diagnostic to include constructor based injections
    // TODO document / investigate other classes derived from ValueType
    // TODo ensure there is a test that uses an object multiple times in the tree.
    // TODO document the fact that member type is based on the type's GetIOCCName() attribute
    // TODO and that generics have the for classname`1[TypeParam]
    // TODO move the majority of unit tests to separate assemblies
    // DONE test generics with multiple parameters
    // DONE test generics with nested parameters
    // TODO ensure where interface->base class->derived class occurs there is no problem with duplication of beans
    // TODO Apply the SimpleIOCContainer to the Calculation Server and Maven docs
    // TODO Release Build
    // TODO improve performance of IOCCObjectTree.CreateObjectTree with respect to dictionary handling
    // TODO make sure that root failure when passing type string is handled via diagnostics and that
    // TODO the explanation is expanded to include that.
    // TODO remove 2-way enumerator
    // TODO Perf
    // TODO Test with nullables
    // TODO make SimpleIOCContainer instance a bean by default.
    // DONE generics for factories
    // TODO decide if scope on factory reference refers to factory or reference to be created
    // TODO it makes no sense for the created object as the scope is under the control of the factory
    // TODO does it make any sense in the case of the factory
    // TODO allow root type a prototype
    // TODO test with no namespace
    // DONE change name to SimpleIOCContainer from SimpleIOCContainer
    // TODO optimised build
    // TODO guard against static constructors
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// constraints:
    ///     1) the object tree (i.e. the program's static model) is required to be static.
    ///     if objects are added to the tree through code at run-time this will not be 
    ///     reflected in the IOC container.
    ///     2) The root class has to be visible to the caller of GetOrCreateObjectTree.
    ///     2a) The root of the tree cannot be specified using reflection.  I'll probably regret that.
    ///     3) static classes and members are not handled.
    ///     4) If a member is incorrectly marked as [IOCCBeanReference] then
    ///        it will be set to its default value even if it is an initialized member.
    ///     5) There is no way for the root bean to be a prototype
    /// </remarks>
    [IOCCBean]
    public class SimpleIOCContainer
    {
        public enum OS { Any, Linux, Windows, MacOS } OS os = new StdOSDetector().DetectOS();
        public static SimpleIOCContainer Instance { get; } = new SimpleIOCContainer();
        internal const string DEFAULT_PROFILE = "";
        internal const string DEFAULT_BEAN_NAME = "";
        internal const string DEFAULT_CONSTRUCTOR_NAME = "";

        private bool getOrCreateObjectTreeCalled = false;
        private IList<string> assemblyNames = new List<string>();
        private readonly IDictionary<string, IOCObjectTreeContainer> mapObjectTreeContainers 
          = new Dictionary<string, IOCObjectTreeContainer>();
        // the key in the objects created so far map comprises 2 types.  The first is the
        // intended concrete type that will be instantiated.  This works well for
        // non-generic types but for generics the concrete type, which is taken from the typeMap,
        // is a generic type definition.  The builder needs to lay its hands on the type argument
        // to substitute for the generic parameter.  The second type (beanReferenceType) which
        // has been taken from the member information of the declaring task provides the generic argument
        IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
            new Dictionary<(Type, string), object>();

        private bool excludeRootAssembly;
        private IDictionary<(Type beanType, string beanName), Type> typeMap;

        private class AssemblyNameComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return base.GetHashCode();
            }
        }
        /// <summary>
        /// for testing only
        /// </summary>
        internal SimpleIOCContainer()
        {
        }

        /// <example>SetAssemblies( true, "MyApp", "MyLib")</example>
        /// <param name="excludeRootAssembly">By default the assembly containing the type
        /// passed to GetOrCreateObjectTree() is included automatically.
        /// Pass true here to ensure it is not scanned for beans.
        /// Note that if you include the root assembly in the list
        /// of assemblies then the excludeRootAssembly flag is ignored.
        /// Note that if a string containing the root type is passed
        /// to GetOrCreateObjectTree() then the system behaves as if
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
            if (getOrCreateObjectTreeCalled)
            {
                throw new InvalidOperationException(
                  "SetAssemblies has been called after GetOrCreateObjectTree."
                  + "  This is not permitted.");
            }
            this.assemblyNames = assemblyNames.ToList();
        }
        // TODO complete the documentation item 3 below if and when factory types are implemented
        // TODO handle situation where there is no console window
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        /// top of the tree - as instantiated by rootType
        /// It does not affect the rest of the tree.  The other nodes on the tree will
        /// honour the Scope property of [IOCCBeanReference]</param>
        /// <returns>an ojbect of root type</returns>
        public TRootType GetOrCreateObjectTree<TRootType>(string profile = DEFAULT_PROFILE
           , string beanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            IOCCDiagnostics diagnostics = null;
            TRootType rootObject = default(TRootType);
            try
            {
                diagnostics = new DiagnosticBuilder().Diagnostics;
                rootObject = GetOrCreateObjectTreeEx<TRootType>(ref diagnostics, profile, beanName, rootConstructorName, scope);
            }
            finally
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
            return rootObject;
        }

        /// <param name="rootTypeName">provided by caller - <see cref="AreTypeNamesEqualish"/></param>
        /// <param name="diagnostics"></param>
        /// <param name="profile"></param>
        /// <param name="rootBeanName">an SimpleIOCContainer type spec in the form "MyNameSpace.MyClass"
        ///     or "MyNameSpace.MyClass&lt;MyActualParam&gt" or
        ///     where inner classes are involved "MyNameSpace.MyClass+MyInnerClass"</param>
        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        /// top of the tree - as instantiated by rootTypeName
        /// It does not affect the rest of the tree.  The other nodes on the tree will
        /// honour the Scope property of [IOCCBeanReference]</param>
        /// <returns></returns>
        public object GetOrCreateObjectTree(string rootTypeName, out IOCCDiagnostics diagnostics
          ,string profile = DEFAULT_PROFILE, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME
          , BeanScope scope = BeanScope.Singleton)
        {
            diagnostics = new DiagnosticBuilder().Diagnostics;
            IList<Assembly> assemblies = AssembleAssemblies(assemblyNames);
            typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(assemblies
              , ref diagnostics, profile, os);
            (Type rootType, string beanName) = typeMap.Keys.FirstOrDefault(k => AreTypeNamesEqualish(k.beanType.FullName, rootTypeName));
            if (rootType == null)
            {
                throw new IOCCException($"Unable to find a type in assembly {assemblyNames.ListContents()} for {rootTypeName}{Environment.NewLine}Remember to include the namespace", diagnostics);
            }
            IOCObjectTreeContainer container;
            if (mapObjectTreeContainers.ContainsKey(profile))
            {
                container = mapObjectTreeContainers[profile];
            }
            else
            {
                container = new IOCObjectTreeContainer(profile, typeMap);
            }
            var rootObject = container.GetOrCreateObjectTree(
              rootType, ref diagnostics, rootBeanName, rootConstructorName, scope, mapObjectsCreatedSoFar);
            if (rootObject == null && diagnostics.HasWarnings)
            {
                throw new IOCCException("Failed to create object tree - see diagnostics for details", diagnostics);
            }
            return rootObject;
        }

        /// <param name="scope">scope refers to the scope of the root bean i.e. the
        /// top of the tree - as instantiated by rootType
        /// It does not affect the rest of the tree.  The other nodes on the tree will
        /// honour the Scope property of [IOCCBeanReference]</param>
        public TRootType GetOrCreateObjectTree<TRootType>(out IOCCDiagnostics diagnostics, string profile = DEFAULT_PROFILE, string rootBeanName = DEFAULT_BEAN_NAME, string rootConstructorName = DEFAULT_CONSTRUCTOR_NAME, BeanScope scope = BeanScope.Singleton)
        {
            diagnostics = new DiagnosticBuilder().Diagnostics;
            return GetOrCreateObjectTreeEx<TRootType>(ref diagnostics, profile, rootBeanName, rootConstructorName, scope);
        }

        /// <summary>
        /// <see cref="GetOrCreateObjectTree"/>
        /// this overload does not print out the diagnostics
        /// </summary>
        /// <param name="diagnostics">This overload exposes the diagnostics object to the caller</param>
        /// <param name="profile"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="scope"></param>
        private TRootType GetOrCreateObjectTreeEx<TRootType>(ref IOCCDiagnostics diagnostics, string profile, string rootBeanName, string rootConstructorName, BeanScope scope)
        {
            return GetOrCreateObjectTreeExEx<TRootType>(ref diagnostics, profile, rootBeanName, rootConstructorName, scope);
        }

        private TRootType GetOrCreateObjectTreeExEx<TRootType>(ref IOCCDiagnostics diagnostics, string profile, string rootBeanName, string rootConstructorName, BeanScope scope)
        {
            getOrCreateObjectTreeCalled = true;
            if (!excludeRootAssembly)
            {
                assemblyNames.Add(typeof(TRootType).Assembly.GetName().Name);
            }
            IList<Assembly> assemblies = AssembleAssemblies(assemblyNames);
            typeMap = new TypeMapBuilder().BuildTypeMapFromAssemblies(assemblies
              , ref diagnostics, profile, os);
            IOCObjectTreeContainer container;
            if (mapObjectTreeContainers.ContainsKey(profile))
            {
                container = mapObjectTreeContainers[profile];
            }
            else
            {
                container = new IOCObjectTreeContainer(profile, typeMap);
            }
            var rootObject = container.GetOrCreateObjectTree(typeof(TRootType), ref diagnostics, rootBeanName, rootConstructorName, scope, mapObjectsCreatedSoFar);
            if (rootObject == null && diagnostics.HasWarnings)
            {
                throw new IOCCException("Failed to create object tree - see diagnostics for details", diagnostics);
            }
            Assert(rootObject is TRootType);
            return (TRootType)rootObject;
        }
        struct st
        {
            private int a;
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
    }       // SimpleIOCContainer
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
              == typeof(IOCCBeanReferenceAttribute) &&
              (ca as IOCCBeanReferenceAttribute).Factory != null);
        }

        public static IOCCBeanReferenceAttribute GetBeanReferenceAttribute(this MemberInfo type)
        {
            return (IOCCBeanReferenceAttribute)type.GetCustomAttributes().Where(
                ca => ca is IOCCBeanReferenceAttribute).FirstOrDefault();
        }
        public static IOCCBeanReferenceAttribute GetBeanReferenceAttribute(this ParameterInfo type)
        {
            return (IOCCBeanReferenceAttribute)type.GetCustomAttributes().Where(
                ca => ca is IOCCBeanReferenceAttribute).FirstOrDefault();
        }
    }
}