using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal static class IOCCExtensions
    {
        /// <summary>
        /// The library has a strong dependency on uniquely identifying the type of objects.
        /// This routine supports that.
        /// </summary>
        /// <param name="type">Typeically a bean or a bean reference - but can be anything</param>
        /// <returns>combines type fullname generic parameters, type arguments</returns>
        public static string GetIOCCName(this Type type)
        {
            return type.FullName;
        }
    }
}