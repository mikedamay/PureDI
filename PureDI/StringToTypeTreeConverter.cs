using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace PureDI
{
    internal class StringToTypeTreeConverter
    {
        public TypeNameTree Convert(string myClass)
        {
            return new TypeNameTree(myClass.Replace(" ", "").Replace("\t",""));
        }
    }
}