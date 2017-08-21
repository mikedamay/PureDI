using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [Bean]
    public class FactoryPrototype : IResultGetter
    {
        [BeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int firstNumber;
        [BeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int secondNumber;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.FirstNumber = firstNumber;
            eo.SecondNumber = secondNumber;
            return eo;
        }
    }
    [Bean]
    public class MyPrototyeFactory : IFactory
    {
        private int accumulator;

        public object Execute(BeanFactoryArgs args)
        {
            accumulator++;
            return accumulator;

        }
    }
}