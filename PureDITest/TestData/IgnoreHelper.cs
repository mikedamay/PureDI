using PureDI;
using PureDI.Attributes;
using MyIgnoredReference = IOCCTest.MyIgnoredReference;

namespace IOCCTest.TestData
{
    [Bean]
    public class IgnoreHelper : MyIgnoredReference
    {
        
    }
}