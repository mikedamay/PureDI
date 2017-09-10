using System;
using System.IO;
using System.Reflection;
using static com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    [Bean]
    public class ResourceFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            object[] @params = (object[])args.FactoryParmeter;
            Assert(@params.Length == 2);
            Assert(@params[0] is Type);
            Assert(@params[1] is String);
            Assembly assembly = (@params[0] as Type).Assembly;
            string location = @params[1] as String;
            using (Stream s = assembly.GetManifestResourceStream(location))
                using (StreamReader sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            
        }
    }
}