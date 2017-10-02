using System.Dynamic;
using PureDI;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [Ignore]
    public abstract class ActualFactoryBase : IFactory
    {
        public abstract (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args);
    }
    [Bean(Name="MyFactory")]
    public class ActualFactory : ActualFactoryBase
    {
        public override (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args) => (42, injectionState);
    }
    [Bean]
    public class AlternateFactory : ActualFactoryBase
    {
        public override (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args) => (24, injectionState);
    }
    [Bean]
    public class FactoryWithName : IResultGetter
    {
        [BeanReference(Name = "MyFactory", Factory = typeof(ActualFactory))] private int mysteryNumber;

        FactoryWithName()
        {
            mysteryNumber = 0;
        }
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MysteryNumber = mysteryNumber;
            return eo;
        }
    }
}