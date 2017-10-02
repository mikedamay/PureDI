using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// The standard exception thrown by the library
    /// if a fatal error occurs during dependency injection.
    /// 
    /// Callers should also call Diagnostics.HasWarnings to
    /// check on the health of injections.  A return value of true
    /// may (but not necessarily) indicate a future fatal problem
    /// for the call application.
    /// </summary>
    public class DIException : Exception
    {
        /// <summary>
        /// An accumulation of all diagnostics up to the point the
        /// exception was thrown.
        /// 
        /// Call Diagnostics.ToString() or Diagnostics.AllToString()
        /// to provide some support in investigating the cause
        /// of the exception.
        /// </summary>
        public Diagnostics Diagnostics { get; } = null;
        /// <summary>
        /// the main exception exposed to library users.  Typically
        /// an exception is thrown if the "root" object cannot
        /// be instantiated.  Where the root object is not involved
        /// then for the most part no exception is thrown, any
        /// problems that occur being recorded as diagnostis.
        /// </summary>
        /// <param name="message">some text helpful to the library user</param>
        /// <param name="diagnostics">accumulated diagnostics to this point in the injection process</param>
        internal DIException(string message, Diagnostics diagnostics) : base(message)
        {
            this.Diagnostics = diagnostics;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">some text helpful to the library user</param>
        /// <param name="innerException">root cause</param>
        /// <param name="diagnostics">accumulated diagnostics to this point in the injection process</param>
        internal DIException(string message, Exception innerException, Diagnostics diagnostics)
          : base(message, innerException)
        {
            this.Diagnostics = diagnostics;
        }
   }
    /// <summary>
    /// Error indicating some problem with the implementation of the library.
    /// Some code contract has been violated
    /// </summary>
    public class IOCCInternalException : Exception
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