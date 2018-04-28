using PureDI.Attributes;

namespace IOCCTest.ClassScraperTestCode
{
    public class DuplicateConstructors
    {
        [Constructor]
        public DuplicateConstructors()
        {
            
        }

        [Constructor]
        public DuplicateConstructors(int ii)
        {
            
        }
    }
}