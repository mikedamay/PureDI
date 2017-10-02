using PureDI;
using MyIgnoredReference = IOCCTest.MyIgnoredReference;

namespace IOCCTest.TestData
{
    [Bean]
    public class IgnoreHelper : MyIgnoredReference
    {
        
    }
}