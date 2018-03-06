using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;
using PureDI.Attributes;

namespace IOCCTest.AlreadyInitialisedTestData
{
    [Bean]
    public class AlreadyInitialised : IResultGetter
    {
        [BeanReference] private SomeClass someClass = new SomeClass("set-by-constructor");
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.SomeClass = someClass;
            return eo;
        }
    }
    [Bean]
    public class SomeClass
    {
        public readonly string someValue = "original";

        public SomeClass(string val)
        {
            this.someValue = val;
        }

        public SomeClass()
        {
            this.someValue = "overwritten";
        }
    }
}