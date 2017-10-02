using System;

namespace PureDI.Tree
{
    internal class IOCCNoArgConstructorException : Exception
    {
        public string Class { get; }
        public IOCCNoArgConstructorException(string _class)
        {
            Class = _class;
        }
    }
}