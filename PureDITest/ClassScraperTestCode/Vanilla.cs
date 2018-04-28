using PureDI.Attributes;

namespace IOCCTest.ClassScraperTestCode
{
    public class Vanilla
    {
        [BeanReference] private SomeClass _someClass = null;
        [BeanReference] private SomeClass3 _someClass3 = null;
        
        [Constructor]
        public Vanilla([BeanReference] SomeClass someClass)
        {
            _ = _someClass;
            _ = _someClass3;
        }

    }

    internal class SomeClass3
    {
    }

    public class SomeClass
     {
     }
}