using System;
using System.Dynamic;
using IOCCTest.TestCode;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.CycleGuardTestData
{
    [Bean]
    public class DiverseCycles : IResultGetter
    {
        [BeanReference(Factory = typeof(DiversFactory), FactoryParameter = typeof(DiverseA))]
        private DiverseA diverseA = null;
        [BeanReference(Factory = typeof(DiversFactory), FactoryParameter = typeof(DiverseB))]
        private DiverseB diverseB = null;
        [BeanReference(Factory = typeof(DiversFactory), FactoryParameter = typeof(DiverseC))]
        private DiverseC diverseC = null;
        [BeanReference(Factory = typeof(DiversFactory), FactoryParameter = typeof(DiverseD))]
        private DiverseD diverseD = null;
        public dynamic GetResults()
        {
            dynamic o = new ExpandoObject();
            o.DiverseA = diverseA;
            o.DiverseB = diverseB;
            o.DiverseC = diverseC;
            o.DiverseD = diverseD;
            return o;
        }
    }

    [Bean]
    internal class DiversFactory : IFactory
    {
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            var pdi = new PDependencyInjector();
            if (args.FactoryParmeter as Type == typeof(DiverseA))
            {
                return pdi.CreateAndInjectDependencies<DiverseA>(injectionState);
            }
            else if (args.FactoryParmeter as Type == typeof(DiverseB))
            {
                return pdi.CreateAndInjectDependencies(args.FactoryParmeter as Type, injectionState);
            }
            else if (args.FactoryParmeter as Type == typeof(DiverseC))
            {
                return pdi.CreateAndInjectDependencies((args.FactoryParmeter as Type).FullName, injectionState);
            }
            else
            {
                var diverseD = new DiverseD();
                return pdi.CreateAndInjectDependencies(diverseD
                  ,PureDI.Common.Constants.DefaultBeanName, injectionState);
            }
        }
    }

    [Bean]
    internal class DiverseD
    {
        [BeanReference] private DiverseCycles cycles = null;

        public DiverseD()
        {
            _ = cycles;
        }
    }

    [Bean]
    internal class DiverseC
    {
        [BeanReference] private DiverseCycles cycles = null;

        private DiverseC()
        {
            _ = cycles;
        }
    }

    [Bean]
    internal class DiverseB
    {
        [BeanReference] private DiverseCycles cycles = null;

        private DiverseB()
        {
            _ = cycles;
        }
    }
    [Bean]
    internal class DiverseA
    {
        [BeanReference] private DiverseCycles cycles = null;

        private DiverseA()
        {
            _ = cycles;
        }
    }

}