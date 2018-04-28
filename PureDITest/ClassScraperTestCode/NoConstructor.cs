using PureDI.Attributes;

namespace IOCCTest.ClassScraperTestCode
{
    public class NoConstructor
    {
        [Constructor]
        private NoConstructor(int ii)
        {
            _ = ii;
        }
    }
}