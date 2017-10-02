using System.Collections.Generic;
using System.IO;
using System.Linq;
using PureDI;

namespace SimpleIOCCDocumentor
{
    internal interface IPropertyMap
    {
        object Map(string key);
        object Map(string key, object @default);
    }
    [Bean(Profile = "noop")]
    public class NoopPropertyMap : IPropertyMap
    {
        public object Map(string key) => null;
        public object Map(string key, object @default) => @default;
    }

    [Bean]
    internal class Mapper : IPropertyMap
    {
        private readonly IDictionary<string, object> kvs;
        public Mapper()
        {
            kvs = new(string key, object value)[] { ("", "") }.ToDictionary(p => p.key, p => p.value);
        }

        public object Map(string key)
        {
            if (kvs.ContainsKey(key))
            {
                return kvs[key];
            }
            return key;
        }
        public object Map(string key, object @default)
        {
            if (kvs.ContainsKey(key))
            {
                return kvs[key];
            }
            return @default;
        }
    }

    [Bean(Profile = "authoring")]
    internal class DocumentMapper : IPropertyMap
    {
        public object Map(string key)
        {
            const string root = "../";
            return Path.Combine(root, Path.ChangeExtension(key, null).Replace(".", "/")) + ".xml";
        }

        public object Map(string key, object @default) => @default;
    }


}