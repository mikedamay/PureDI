using PureDI.Attributes;

namespace PureDITest.RootObjectTestCode
{
    public class NamedConstructor
    {
        [Constructor(Name="MyConstructor")]
        public NamedConstructor(
            [BeanReference] MyParam myParam)
        {
            
        }
    }
    [Bean]
    public class MyParam
    {
    }

    [Bean]
    public class AnotherClass
    {
        [BeanReference(ConstructorName = "MyConstructor")] public NamedConstructor named;
    }
}