using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class StringToTypeTreeConverter
    {
        public TypeNameTree Convert(string myClass)
        {
            return new TypeNameTree(myClass.Replace(" ", "").Replace("\t",""));
        }
    }
}