using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [Bean]
    public class FactoryPrototype : IResultGetter
    {
        [BeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int firstNumber = 0;
        [BeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int secondNumber = 0;

        FactoryPrototype()
        {
            firstNumber = 0;
            secondNumber = 0;

        }
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

        public (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            accumulator++;
            return (accumulator, injectionState);

        }
    }
}