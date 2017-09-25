using System;

namespace com.TheDisappointedProgrammer.IOCC.Common
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
#if true
        public static readonly string ResourcePrefix 
          = typeof(Common).Assembly.GetName().Name;
#else
        // when the project was originally created
        // by VS as a .net framework class library
        // the location of resources was as below.
        // Having switched to netcoreapp2_0 and
        // back to net47 the resources are located
        // as above.  Presumably, the original
        // csproj overrode some default setting.
        public static readonly string ResourcePrefix 
          = typeof(SimpleIOCContainer).Namespace;
#endif
    }
}