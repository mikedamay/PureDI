using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC.Tree;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// contains one or more trees for a specific profile
    /// </summary>
    /// <remarks>
    /// The generic parameter TRootType determines which tree will be returned
    /// </remarks>
    internal class ObjectTreeContainer
    {
        private string profile;
        private IDictionary<(Type, string), ObjectTree> mapTrees = new Dictionary<(Type, string), ObjectTree>();
        private readonly IDictionary<(Type type, string name), Type> typeMap;
 
        internal ObjectTreeContainer(string profile, IDictionary<(Type, string), Type> typeMap)
        {
            this.profile = profile;
            this.typeMap = typeMap;
        }
        public object CreateAndInjectDependencies(Type rootType
          , IOCCDiagnostics diagnostics, string rootBeanName, string rootConstructorName
          , BeanScope scope, IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            ObjectTree tree;
            if (mapTrees.ContainsKey((rootType, rootBeanName)))
            {
                tree = mapTrees[(rootType, rootBeanName)];
            }
            else
            {
                tree = new ObjectTree(profile, typeMap);
                mapTrees[(rootType, rootBeanName)] = tree;
                 
            }
            return tree.CreateAndInjectDependencies(rootType, diagnostics
              ,rootBeanName, rootConstructorName, scope, mapObjectsCreatedSoFar);
        }
    }
}
