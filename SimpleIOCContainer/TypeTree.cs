using System.Collections.Generic;
using System.Runtime.Hosting;
using System.Text;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// takes a string like "MyClass&lt;MyClass2&gt; and builds a tree
    /// TreeMap of "MyClass`1" -> TreeMap of "MyClass2"
    /// </summary>
    internal class TypeTree
    {
        private readonly List<TypeTree> genericArguments = new List<TypeTree>();
        private string typeFullName;
        /// <param name="typeSpec">namespace.classname&lt;genericAgrument&gt;</param>
        public TypeTree(string typeSpec) : this(new TwoWayEnumerator<char>(typeSpec.GetEnumerator())
          , new TypeTree(new TwoWayEnumerator<char>("".GetEnumerator()), null))
        {
            // embedded spaces must be dealt with by caller
            System.Diagnostics.Debug.Assert(!typeSpec.Contains(" "));
            System.Diagnostics.Debug.Assert(!typeSpec.Contains("\t"));
        }

        private TypeTree(ITwoWayEnumerator<char> typeSpec, TypeTree parent)
        {
            ProcessTypeSpec(typeSpec, parent);
        }
        public string TypeFullName => typeFullName;
        public List<TypeTree> GenericArguments => genericArguments;

        private void ProcessTypeSpec(ITwoWayEnumerator<char> typeSpec, TypeTree parent)
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
                        typeSpec.MoveNext();    // eat the trailing ">"
                        return;
                    case ',':
                        this.typeFullName = sb.ToString();
                        sb.Clear();
                        parent.GenericArguments.Add(this);
                        return;
                    case '>':
                        if (sb.ToString() != string.Empty)
                        {
                            this.typeFullName = sb.ToString();
                            sb.Clear();
                            parent.GenericArguments.Add(this);
                        }
                        return;
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
        }

        private void AddChildren(ITwoWayEnumerator<char> typeSpec)
        {
            do
            {
                new TypeTree(typeSpec, this);
                if (typeSpec.Current != ',')
                {
                    return;     // no more children
                }
            } while (true);
        }
    }
}