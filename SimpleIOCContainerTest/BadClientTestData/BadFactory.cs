using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.BadClientTestData
{
    [Bean]
    public class BadFactory
    {
        [BeanReference(Factory=typeof(ActualBadFactory))] private string actual = null;

        BadFactory()
        {
            var x = actual;
        }
        [Bean]
        internal class ActualBadFactory : IFactory
        {
            public (object bean, InjectionState injectionState) Execute(InjectionState injectionState
                , BeanFactoryArgs args)
            {
                int x = ((string)null).Length;
                return ("", injectionState);
            }
        }

    }
}