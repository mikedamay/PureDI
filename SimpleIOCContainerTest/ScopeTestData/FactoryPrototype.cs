using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.ScopeTestData
{
    [IOCCBean]
    public class FactoryPrototype : IResultGetter
    {
        [IOCCBeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int firstNumber;
        [IOCCBeanReference(Factory = typeof(MyPrototyeFactory), Scope = BeanScope.Prototype)] private int secondNumber;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.FirstNumber = firstNumber;
            eo.SecondNumber = secondNumber;
            return eo;
        }
    }
    [IOCCBean]
    public class MyPrototyeFactory : IOCCFactory
    {
        private int accumulator;

        public object Execute(BeanFactoryArgs args)
        {
            accumulator++;
            return accumulator;

        }
    }
}