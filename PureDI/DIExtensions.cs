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
        public static string GetSafeFullName(this Type type)
        {
            return type.FullName ?? $"{type.Namespace}.{type.Name}";
                    // interface generic definitions return null from FullName
                    // although there are many cases where a type should return
                    // null as FullName this does not appear to meet the criteria
                    // it looks like a bug to me
        }
    }
}