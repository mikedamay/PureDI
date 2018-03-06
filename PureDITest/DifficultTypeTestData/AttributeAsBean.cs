using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using PureDI;
using PureDI.Attributes;
using PureDI.Tree;
using IOCCTest.TestCode;

namespace IOCCTest.DifficultTypeTestData
{
    [Bean]
    public class SomeAttribute : Attribute
    {
        [BeanReference] public SomeClass Something;
    }

    [Bean]
    public class AttributeAsBean : IResultGetter
    {
        public int? GetSomething()
        {
            return null;

        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeOtherValue = GetSomething();
            return eo;
        }
        [Some]
        public int? SomeOtherValue;
    }

    [Bean]
    public class SomeClass
    {
        public int SomeValue = 42;
    }
}