using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    // TODO guard against circular references
    // TODO handle nexted classes
    // TODO handle structs
    // TODO handle properties
    // TODO object factories
    // TODO handle multiple assemblies
    // TODO references held in tuples
    // TODO references held in embedded structs 
    // TODO references held as objects
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// constraints:
    ///     the object tree (i.e. the program's static model) is required to be static.
    ///     if objects are added to the tree through code at run-time this will not be 
    ///     reflected in the IOC container.
    /// </remarks>
    public class IOCC
    {
        public enum OS { Any, Linux, Windows, MacOS } OS os = new StdOSDetector().DetectOS();
        public static IOCC Instance { get; } = new IOCC();
        internal const string DEFAULT_PROFILE = "";
        internal const string DEFAULT_DEPENDENCY_NAME = "";

        private bool getOrCreateObjectTreeCalled = false;
        private IList<string> assemblyNames = new List<string>();
        private readonly IDictionary<string, IOCObjectTreeContainer> mapObjectTreeContainers 
          = new Dictionary<string, IOCObjectTreeContainer>();

        private readonly IDictionary<(Type, string, string), Type> typeMap = new Dictionary<(Type, string, string), Type>()
        {
            { (typeof(TestIOCC), "", ""), typeof(TestIOCC)}
            ,{ (typeof(ChildOne), "", ""), typeof(ChildOne)}

        };

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

        private IOCC()
        {        
        }

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
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <returns>an ojbect of root type</returns>
        public TRootType GetOrCreateObjectTree<TRootType>(string profile = DEFAULT_PROFILE)
        {
            getOrCreateObjectTreeCalled = true;
            IList<Assembly> assemblies = AssembleAssemblies(assemblyNames, typeof(TRootType).Assembly);
            IOCObjectTreeContainer container;
            if (mapObjectTreeContainers.ContainsKey(profile))
            {
                container = mapObjectTreeContainers[profile];
            }
            else
            {
                container = new IOCObjectTreeContainer(profile, typeMap);
            }
            return container.GetOrCreateObjectTree<TRootType>();
        }
        /// <summary>
        /// builds list of all the assemblies involved in the dependency tree
        /// </summary>
        /// <param name="assemblyNames">list of names provided by caller.
        /// This should include the names of all assemblies containing dependencies (concrete classes)
        /// involved in the object model of this app.  Optionally this can include the assembly
        /// of the root class in the tree</param>
        /// <param name="rootAssembly">The assembly which contains the root class</param>
        /// <returns></returns>
        private IList<Assembly> AssembleAssemblies(IList<string> assemblyNames, Assembly rootAssembly)
        {
            ISet<string> assemblyNamesSet = new HashSet<string>(assemblyNames, new AssemblyNameComparer());
            ISet<Assembly> assembliesSet = new HashSet<Assembly>();
            assembliesSet.Add(rootAssembly);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(assembly.GetName().Name);
                if (assemblyNamesSet.Contains(assembly.GetName().Name))
                {
                    assembliesSet.Add(assembly);
                }
            }
            return assembliesSet.ToList();
        }
    }
}
