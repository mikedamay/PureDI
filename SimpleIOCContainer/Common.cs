using System;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal static class Common
    {
        public static bool Assert(bool expr)
        {
            //System.Diagnostics.Debug.Assert(expr);
                // can't do this as it upsets unit tests
            if (!expr)
            {
                throw new Exception("assertion failure");
            }
            return true;
        }
#if NETSTANDARD2_0
        public static readonly string ResourcePrefix 
          = typeof(Common).Assembly.GetName().Name;
#else
        public static readonly string ResourcePrefix 
          = typeof(SimpleIOCContainer).Namespace;
#endif
    }
}