using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.CaseSensitivityTestData
{
    [Bean]
    public class Interface : IResultGetter
    {
        [BeanReference] private ISimple UpperCase = null;
        [BeanReference(Name="lowercase")] private ISimple LowerCase = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.UpperCase = UpperCase.val;
            eo.LowerCase = LowerCase.val;
            return eo;
        }

    }

    public interface ISimple
    {
        string val { get; }
    }

    [Bean]
    public class Simple4 : ISimple
    {
        public string val => "uppercase";
    }
    [Bean(Name="lowercase")]
    public class simple4 : ISimple
    {
        public string val => "lowercase";
    }
}