using System;
using System.Collections.Generic;
using System.Linq;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// compares a type in the assembly with an ioc type spec
    /// such as "MyClass&lt;string&gt;"
    /// </summary>
    internal class TypeMatcher
    {
        public bool Match(Type type, string typeSpec)
        {
            TypeNameTree typeNameTree = new StringToTypeTreeConverter().Convert(typeSpec);
            return Match(type, new TypeNameTree(""));
        }

        internal bool Match(Type type, TypeNameTree typeNameTree)
        {
            // The full name returned by constructed generic types includes details
            // of the parameter arguments inluding assembly and version.  The name has to be curtailed
            // e.g. MyClass`1[[System.Int32, mscorlib, version=4.0.0....]] -> MyClass`1 
            if (new string(type.FullName.TakeWhile(c => c != '[').ToArray()) == typeNameTree.TypeFullName)
            {
                IEnumerator<TypeNameTree> typeTreeIter = typeNameTree.GenericArguments.OrderBy(tt => tt.TypeFullName)
                    .GetEnumerator();
                typeTreeIter.MoveNext();
                foreach (Type childType in type.GenericTypeArguments.OrderBy(t => t.FullName))
                {
                    if (Match(childType, typeTreeIter.Current))
                    {
                        typeTreeIter.MoveNext();
                    }
                    else
                    {
                        return false;   // children don't match
                    }
                }
                return true;            // current type tree and all its children match
            }
            return false;               // current tree type doesn't match
        }
    }
}