using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
                IEnumerator<TypeTree> typeTreeiter = typeTree.GenericArguments.OrderBy(tt => tt.TypeFullName)
                    .GetEnumerator();
                typeTreeiter.MoveNext();
                foreach (Type childType in type.GenericTypeArguments.OrderBy(t => t.FullName))
                {
                    if (Match(childType, typeTreeiter.Current))
                    {
                        typeTreeiter.MoveNext();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}