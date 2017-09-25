using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class IOCCException : Exception
    {
        public IOCCDiagnostics Diagnostics { get; } = null;
        public IOCCException(string message, IOCCDiagnostics diagnostics) : base(message)
        {
            this.Diagnostics = diagnostics;
        }

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