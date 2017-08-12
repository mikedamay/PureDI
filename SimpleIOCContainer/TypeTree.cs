using System.Collections.Generic;
using System.Runtime.Hosting;
using System.Text;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class TypeTree
    {
        private readonly List<string> genericArguments = new List<string>();
        private string typeFullName;
        /// <param name="typeSpec">namespace.classname&lt;genericAgrument&gt;</param>
        public TypeTree(string typeSpec) : this(typeSpec, null)
        {
        }

        private TypeTree(string typeSpec, TypeTree parent)
        {
            ProcessTypeSpec(typeSpec, null);
        }
        public string TypeFullName => typeFullName;
        public List<string> GenericArguments => genericArguments;

        private void ProcessTypeSpec(string typeSpec, TypeTree parent)
        {
            int argCtr = 0;
            StringBuilder sb = new StringBuilder();
            IEnumerable<char> chars = typeSpec;
            foreach (char ch in chars)
            {
                switch (ch)
                {
                    case '<':
                        this.typeFullName = sb.ToString();
                        sb.Clear();
                        break;
                    case ',':
                    case '>':
                        this.GenericArguments.Add(sb.ToString());
                        sb.Clear();
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            if (sb.ToString() != string.Empty)
            {
                this.typeFullName = sb.ToString();
            }
        }
    }
}