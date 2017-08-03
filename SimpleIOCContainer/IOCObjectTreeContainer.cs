﻿using System;
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
        private IDictionary<Type, IOCObjectTree> mapTrees = new Dictionary<Type, IOCObjectTree>();
        private readonly IDictionary<(Type type, string name), Type> typeMap;
 
        internal IOCObjectTreeContainer(string profile, IDictionary<(Type, string), Type> typeMap)
        {
            this.profile = profile;
            this.typeMap = typeMap;
        }
        public TRootType GetOrCreateObjectTree<TRootType>(ref IOCCDiagnostics diagnostics)
        {
            Type rootType = typeof(TRootType);
            IOCObjectTree tree;
            if (mapTrees.ContainsKey(rootType))
            {
                tree = mapTrees[rootType];
            }
            else
            {
                tree = new IOCObjectTree(profile, typeMap);
                mapTrees[rootType] = tree;
                 
            }
            return tree.GetOrCreateObjectTree<TRootType>(ref diagnostics);
        }
    }
}
