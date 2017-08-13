using System;
using System.Collections.Generic;
using System.Linq;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest
{
    /// <summary>
    /// compares a type in the assembly with an ioc type spec
    /// such as "MyClass&lt;string&gt;
    /// </summary>
    internal class TypeMatcher
    {
        public bool Match(Type type, string typeSpec)
        {
            TypeTree typeTree = new StringToTypeTreeConverter().Convert(typeSpec);
            return Match(type, new TypeTree(""));
        }

        internal bool Match(Type type, TypeTree typeTree)
        {
            // The full name returned by constructed generic types includes details
            // of the parameter arguments inluding assembly and version.  The name has to be curtailed
            // e.g. MyClass`1[[System.Int32, mscorlib, version=4.0.0....]] -> MyClass`1 
            if (new string(type.FullName.TakeWhile(c => c != '[').ToArray()) == typeTree.TypeFullName)
            {
                if (type.GenericTypeArguments.Length != typeTree.GenericArguments.Count)
                {
                    return false;
                }
                if (type.GenericTypeArguments.Length == 0)
                {
                    return true;
                }
                TypeTree[] typeTrees = new TypeTree[typeTree.GenericArguments.Count];
                typeTree.GenericArguments.CopyTo(typeTrees);
                List<TypeTree> typeTreeChildren = new List<TypeTree>(typeTrees);
                foreach (Type childType in type.GenericTypeArguments)
                {
                    foreach (TypeTree childTypeTree in typeTreeChildren)
                    {
                        if (Match(childType, childTypeTree))
                        {
                            typeTreeChildren.Remove(childTypeTree);
                            if (typeTreeChildren.Count == 0)
                            {
                                return true; // we've matched every type at this level so things must be good
                            }
                            break;
                        }
                    }
                }
            }
            return false;   // failed to find a match for all the children
        }
    }
}