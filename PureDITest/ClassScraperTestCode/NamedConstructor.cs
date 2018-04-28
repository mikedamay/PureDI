using PureDI.Attributes;

namespace IOCCTest.ClassScraperTestCode
{
    public class NamedConstructor
    {
        [Constructor(Name = "MyConstructor")]
        public NamedConstructor([BeanReference] SomeClass2 someClass)
        {
            
        }
    }

    public class SomeClass2
    {
    }
}