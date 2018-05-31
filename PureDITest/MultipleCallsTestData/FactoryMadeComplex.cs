using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class FactoryMadeComplex : IResultGetter
    {
        [BeanReference] private Made made = null;
        public dynamic GetResults()
        {
            _ = made;
            dynamic eo = new ExpandoObject();
            eo.FurthestCtr = Furthest.InstanceCtr;
            return eo;
        }
    }
    [Bean]
    public class Made
    {
        [BeanReference(Factory = typeof(FurtherFactory))]
        private Further further = null;
        [BeanReference(Factory = typeof(FurtherFactory))]
        private Further further2 = null;
        [Constructor]
        public Made([BeanReference(Factory = typeof(FurtherFactory))]
        Further further, [BeanReference(Factory = typeof(FurtherFactory))]
        Further further2)
        {
            _ = this.further2;
            _ = this.further;
            _ = further;
            _ = further2;
        }
    }
    [Bean]
    public class FurtherFactory : IFactory
    {
        [BeanReference] private DependencyInjector injector = null;
        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            return injector.CreateAndInjectDependencies<Further>(injectionState);
        }
    }

    [Bean]
    public class Further : IResultGetter
    {
        [BeanReference] private Furthest furthest = null;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Furthest = furthest;
            return eo;
        }
        
    }
    [Bean]
    internal class Furthest
    {
        public static int InstanceCtr;
        public Furthest()
        {
            InstanceCtr++;
        }
    }
}