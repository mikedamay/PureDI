using PureDI;

namespace IOCCTest.TestData
{
    [Bean]
    public class DuplicateBeans : Interface1
    {
        
    }
    [Bean]
    public class DuplicateBeans2 : Interface1
    {
        
    }

    [Bean]
    public class TrailingBean
    {
        
    }

    interface Interface1
    {
        
    }
}