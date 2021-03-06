﻿using System.Collections.Generic;
using System.Text;

namespace PureDI
{
    /// <summary>
    /// takes a string like "MyClass&lt;MyClass2&gt;" and builds a tree
    /// TreeMap of "MyClass`1" -> TreeMap of "MyClass2"
    /// TODO make this more readable - not sure what happened.
    /// </summary>
    internal class TypeNameTree
    {
        private readonly List<TypeNameTree> genericArguments = new List<TypeNameTree>();
        private string typeFullName;

        /// <param name="typeSpec">namespace.classname&lt;genericAgrument&gt;</param>
        public TypeNameTree(string typeSpec)
        {
            // embedded spaces must be dealt with by caller
            System.Diagnostics.Debug.Assert(!typeSpec.Contains(" "));
            System.Diagnostics.Debug.Assert(!typeSpec.Contains("\t"));
            ProcessTypeSpec(typeSpec.GetEnumerator(), new TypeNameTree("".GetEnumerator(), null, out var dummy));
        }

        private TypeNameTree(IEnumerator<char> typeSpec, TypeNameTree parent, out bool @continue)
        {
            @continue = ProcessTypeSpec(typeSpec, parent);
        }
        public string TypeFullName => typeFullName;
        public List<TypeNameTree> GenericArguments => genericArguments;

        private bool ProcessTypeSpec(IEnumerator<char> typeSpec, TypeNameTree parent)
        {
            StringBuilder sb = new StringBuilder();
            while (typeSpec.MoveNext())
            {
                char ch;
                switch (ch = typeSpec.Current)
                {
                    case '<':
                        parent.GenericArguments.Add(this);
                        AddChildren(typeSpec);
                        this.typeFullName = sb.ToString() + "`" + genericArguments.Count;
                        sb.Clear();
                        bool bResult = typeSpec.MoveNext();    // eat the trailing ">"
                        return bResult;
                    case ',':
                        this.typeFullName = sb.ToString();
                        sb.Clear();
                        parent.GenericArguments.Add(this);
                        return true;
                    case '>':
                        if (sb.ToString() != string.Empty)
                        {
                            this.typeFullName = sb.ToString();
                            sb.Clear();
                            parent.GenericArguments.Add(this);
                        }
                        return true;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            if (sb.ToString() != string.Empty)
            {
                this.typeFullName = sb.ToString();
                parent.GenericArguments.Add(this);
            }
            return false;
        }

        private void AddChildren(IEnumerator<char> typeSpec)
        {
            do
            {
                bool @continue;
                new TypeNameTree(typeSpec, this, out @continue);
                if (!@continue || typeSpec.Current != ',')
                {
                    return;     // no more children
                }
            } while (true);
        }
    }
}