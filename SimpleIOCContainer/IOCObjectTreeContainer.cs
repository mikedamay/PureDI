using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// contains one or more trees for a specific profile
    /// </summary>
    /// <remarks>
    /// The generic parameter TRootType determines which tree will be returned
    /// </remarks>
    internal class IOCObjectTreeContainer
    {
        private string profile;
        private IDictionary<(Type, string), IOCObjectTree> mapTrees = new Dictionary<(Type, string), IOCObjectTree>();
        private readonly IDictionary<(Type type, string name), Type> typeMap;
 
        internal IOCObjectTreeContainer(string profile, IDictionary<(Type, string), Type> typeMap)
        {
            this.profile = profile;
            this.typeMap = typeMap;
        }
        public object GetOrCreateObjectTree(Type rootType
          , ref IOCCDiagnostics diagnostics, string rootBeanName, string rootConstructorName
          , BeanScope scope, IDictionary<Type, object> mapObjectsCreatedSoFar)
        {
            IOCObjectTree tree;
            if (mapTrees.ContainsKey((rootType, rootBeanName)))
            {
                tree = mapTrees[(rootType, rootBeanName)];
            }
            else
            {
                tree = new IOCObjectTree(profile, typeMap);
                mapTrees[(rootType, rootBeanName)] = tree;
                 
            }
            return tree.GetOrCreateObjectTree(rootType, ref diagnostics
              ,rootBeanName, rootConstructorName, scope, mapObjectsCreatedSoFar);
        }
    }
}
