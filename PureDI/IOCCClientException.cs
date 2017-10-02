using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class IOCCClientException : Exception
    {
        public IOCCClientException(string message
          , Exception innerException) : base(message, innerException)
        {
            
        }        
    }
}