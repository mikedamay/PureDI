using System;
using System.IO;
using System.Reflection;
using static com.TheDisappointedProgrammer.IOCC.Common.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    [IOCCIgnore]
    public abstract class ResourceFactoryBase : IFactory
    {
        public string GetResourceAsString(Type assemblyFinder, string resourcePath)
        {
            using (Stream s = assemblyFinder.Assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }
        public virtual object Execute(BeanFactoryArgs args)
        {
            object[] @params = (object[])args.FactoryParmeter;
            Assert(@params.Length == 2);
            Assert(@params[0] is Type);
            Assert(@params[1] is String);
            return GetResourceAsString(@params[0] as Type, @params[1] as String);
            
        }
    
    }
    [Bean]
    public class ResourceFactory : ResourceFactoryBase {
        }
}