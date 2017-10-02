using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class ComplexFactory : IResultGetter
    {
        [BeanReference(Factory = typeof(ChildFactory), FactoryParameter = 1)] private ChildOne childOne = null;
        [BeanReference(Factory = typeof(ChildFactory), FactoryParameter = 2)] private ChildTwo childTwo = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildOne = childOne;
            eo.ChildTwo = childTwo;
            return eo;
        }

    }
    [Bean]
    public class ChildFactory : IFactory
    {
        [BeanReference] private PDependencyInjector pDependencyInjector = null;

        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            switch ((int) args.FactoryParmeter)
            {
                case 1:
                    return pDependencyInjector.CreateAndInjectDependencies<ChildOne>(injectionState);
                case 2:
                    return pDependencyInjector.CreateAndInjectDependencies<ChildTwo>(injectionState);
                default:
                    throw new Exception("Execute failed");

            }
        }
    }
    [Bean]
    internal class ChildOne : IResultGetter
    {
        [BeanReference] private ChildTwo childTwo = null;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.ChildTwo = childTwo;
            return eo;
        }

    }
    [Bean]
    public class ChildTwo
    {
    }
}