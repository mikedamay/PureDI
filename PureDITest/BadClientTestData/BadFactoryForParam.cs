using PureDI;

namespace IOCCTest.BadClientTestData
{
    [Bean]
    public class BadFactoryForParam
    {
        [BeanReference(Factory = typeof(ActualBadFactoryForParam))] private BeanWithConstructor actual = null;
        BadFactoryForParam()      
        {
            var x = actual;
        }
        [Bean]
        internal class ActualBadFactoryForParam : IFactory
        {
            public (object bean, InjectionState injectionState) Execute(InjectionState injectionState
                , BeanFactoryArgs args)
            {
                int x = ((string)null).Length;
                return ("", injectionState);
            }
        }
    }

    [Bean]
    internal class BeanWithConstructor
    {
        [Constructor]
        BeanWithConstructor(
            [BeanReference(Factory = typeof(BadFactoryForParam.ActualBadFactoryForParam))] string something
        )
        {
            
        }
    }
}