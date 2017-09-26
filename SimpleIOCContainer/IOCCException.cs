using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class IOCCException : Exception
    {
        public IOCCDiagnostics Diagnostics { get; } = null;
        /// <summary>
        /// the main exception exposed to library users.  Typically
        /// an exception is thrown if the "root" object cannot
        /// be instantiated.  Where the root object is not involved
        /// then for the most part no exception is thrown, any
        /// problems that occur being recorded as diagnostis.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="diagnostics"></param>
        public IOCCException(string message, IOCCDiagnostics diagnostics) : base(message)
        {
            this.Diagnostics = diagnostics;
        }
        /// <inheritdoc cref="IOCCException(string, IOCCDiagnostics)"/>
        public IOCCException(string message, Exception innerException, IOCCDiagnostics diagnostics)
          : base(message, innerException)
        {
            this.Diagnostics = diagnostics;
        }
   }

    public class IOCCInternalException : Exception
    {
        public IOCCInternalException(string message) : base(message)
        {
        }

        public IOCCInternalException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
       
    }
}