using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class IOCCException : Exception
    {
        public IOCCException(string message) : base(message)
        {
        }

        public IOCCException(string message, Exception innerException)
          : base(message, innerException)
        {
        }
    }
}