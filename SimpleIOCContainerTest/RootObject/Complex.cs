using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.rootBean
{
    [Bean]
    public class ValueFactory : IFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new ValueObject((string) args.FactoryParmeter);
        }
    }
    [Bean]
    public class Complex
    {
        [BeanReference(Factory = typeof(ValueFactory), FactoryParameter = "ValueOne")] public ValueObject value1;
        [BeanReference(Factory = typeof(ValueFactory), FactoryParameter = "ValueTwo")] public ValueObject value2;
    }
    [Bean]
    public class ValueObject
    {
        public string val;
        public ValueObject(string val)
        {
            this.val = val;
        }
    }
}