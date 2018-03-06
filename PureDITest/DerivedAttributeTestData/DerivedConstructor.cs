using System;
using System.Dynamic;
using System.Reflection.Emit;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    public class DerivedConstructAttribute : ConstructorBaseAttribute
    {
        public DerivedConstructAttribute()
        {
            base.Name = "TestConstructor";
        }
    }
    [Bean]
    public class DerivedConstructor : IResultGetter
    {
        [BeanReference(ConstructorName="TestConstructor")]
        private ActualDerivedClass actual;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Stuff = actual?.some?.stuff;
            return eo;
        }
    }
    [Bean]
    public class ActualDerivedClass
    {
        public SomeOtherClass some;
        [DerivedConstruct(Name="TestConstructor")]
        public ActualDerivedClass([BeanReference]SomeOtherClass some)
        {
            this.some = some;
        }
    }
    [Bean]
    public class SomeOtherClass
    {
        public string stuff = "somestuff";
    }

}