using System;

namespace PureDI
{
    internal static class DIExtensions
    {
        /// <summary>
        /// The library has a strong dependency on uniquely identifying the type of objects.
        /// This routine supports that.
        /// </summary>
        /// <param name="type">Typically a bean or a bean reference - but can be anything</param>
        /// <returns>combines type full name generic parameters, type arguments</returns>
        public static string GetIOCCName(this Type type)
        {
            return type.FullName;
        }
    }
}