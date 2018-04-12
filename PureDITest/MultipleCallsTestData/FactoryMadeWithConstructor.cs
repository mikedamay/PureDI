using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.MultipleCallsTestData
{
    [Bean]
    public class FactoryMadeWithConstructor : IResultGetter
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
        [Constructor]
        public Made([BeanReference(Factory=typeof(FurtherFactory))]Further further)
        {
            
        }
    }
    [Bean]
    public class FurtherFactory : IFactory
    {
        [BeanReference] private PDependencyInjector injector = null;
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