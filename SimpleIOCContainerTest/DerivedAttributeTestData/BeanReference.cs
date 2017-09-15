using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property
                    | AttributeTargets.Parameter
        , Inherited = false, AllowMultiple = false)]
    public class DerivedReferenceAttribute : BeanReferenceBaseAttribute
    {
        
    }

    [Bean]
    public class BeanReference : IResultGetter
    {
        [DerivedReference] private Referred referred;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Referred = referred;
            return eo;
        }
    }

    [Bean]
    public class Referred
    {
        
    }
}