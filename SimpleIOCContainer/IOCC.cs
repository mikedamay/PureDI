
using System;
using System.Collections.Generic;

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


        private readonly IDictionary<string, IOCObjectTreeContainer> mapObjectTreeContainers 
          = new Dictionary<string, IOCObjectTreeContainer>();

        private readonly IDictionary<(Type, string, string), Type> typeMap = new Dictionary<(Type, string, string), Type>()
        {
            { (typeof(TestIOCC), "", ""), typeof(TestIOCC)}
            ,{ (typeof(ChildOne), "", ""), typeof(ChildOne)}

        };

        private IOCC()
        {        
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
    }
}
