using System;

namespace PureDI.Tree
{
    internal class NoArgConstructorException : Exception
    {
        public string Class { get; }
        public NoArgConstructorException(string _class)
        {
            Class = _class;
        }
    }
}