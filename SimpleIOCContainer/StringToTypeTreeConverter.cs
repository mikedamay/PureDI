namespace com.TheDisappointedProgrammer.IOCC
{
    internal class StringToTypeTreeConverter
    {
        public TypeTree Convert(string myClass)
        {
            return new TypeTree(myClass);
        }
    }
}