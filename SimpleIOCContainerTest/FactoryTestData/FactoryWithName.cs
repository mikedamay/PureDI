using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCIgnore]
    public abstract class ActualFactoryBase : IFactory
    {
        public abstract object Execute(BeanFactoryArgs args);
    }
    [Bean(Name="MyFactory")]
    public class ActualFactory : ActualFactoryBase
    {
        public override object Execute(BeanFactoryArgs args) => 42;
    }
    [Bean]
    public class AlternateFactory : ActualFactoryBase
    {
        public override object Execute(BeanFactoryArgs args) => 24;
    }
    [Bean]
    public class FactoryWithName : IResultGetter
    {
        [BeanReference(Name = "MyFactory", Factory = typeof(ActualFactory))] private int mysteryNumber;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MysteryNumber = mysteryNumber;
            return eo;
        }
    }
}