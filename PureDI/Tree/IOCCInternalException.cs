using System;

namespace PureDI.Tree
{
    /// <summary>
    /// Error indicating some problem with the implementation of the library.
    /// Some code contract has been violated
    /// </summary>
    internal class IOCCInternalException : Exception
    {
        internal IOCCInternalException(string message) : base(message)
        {
        }

        internal IOCCInternalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
       
    }
}