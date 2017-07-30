using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace com.theDisappointedProgrammer.Drive
{
    public interface OSPlatform
    {    
       string GetHomeDirectoryName();
    }
    internal class WindowsPlatform : OSPlatform
    {
        public string GetHomeDirectoryName()
        {
            try
            {
                string drive = System.Environment.GetEnvironmentVariable("HOMEDRIVE");
                string path = System.Environment.GetEnvironmentVariable("HOMEPATH");
                if (drive != null && path != null)
                {
                    return System.IO.Path.Combine(drive, path);
                }
                throw new Exception("environment variable HOMEDRIVE of HOMEPATH is missing");
 
            }
            catch (SecurityException se)
            {
                throw new Exception(
                  "Possibly attempting to execute in an environment when you are not permissioned"
                  +" for environment variables. In any case something's broken!"
                  , se);
            }
       }
    }
}
