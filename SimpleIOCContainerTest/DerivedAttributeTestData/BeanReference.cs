using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    public class DerivedReferenceAttribute : BeanReferenceAttribute
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