using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.Extensions.Configuration;

namespace SimpleIOCCDocumentor
{
    /// <summary>
    /// usage:
    /// [Config(Key: "theUltimateQuestion"] private int someValue;  // someValue will get set to 42
    /// </summary>

    public interface IConfig
    {
        object GetValue(string key, object defaultValue);
    }
    [Bean]
    public class GenericConfig : IConfig
    {
        private IDictionary<string, object> map;
        /// <summary>
        /// in order to use the constructor this object needs to be instantiated
        /// before everything else.  I think constructor parameters will
        /// help alleviate this.
        /// </summary>
        /// <param name="entries">set of key-value pairs to provide
        /// configuration mapping</param>
        public GenericConfig(params (string key, object value)[] entries)
        {
            map = entries.ToDictionary((kv) => kv.key, (kv) => kv.value);
        }
        public object GetValue(string key, object defaultValue)
        {
            if (map.ContainsKey(key))
            {
                return map[key];
            }
            return defaultValue;
        }
    }

    [Bean]
    public class GenericConfigFactory : IFactory
    {
        [BeanReference] private IConfig config = null;

        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            const int KEY_PARAM = 0;
            const int VALUE_PARAM = 1;
            object[] @params = (object[]) args.FactoryParmeter;
            return (config.GetValue((string)@params[KEY_PARAM], @params[VALUE_PARAM]), injectionState);
        }
    }

    public class ConfigAttribute : BeanReferenceBaseAttribute
    {
        public ConfigAttribute(string Key, object Default = null)
        {
            FactoryParameter = new[] {Key, Default};
            Factory = typeof(GenericConfigFactory);
        }
    }
}
