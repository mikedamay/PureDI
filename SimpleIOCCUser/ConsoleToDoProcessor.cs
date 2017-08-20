using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [IOCCBean]
    public class ConsoleToDoProcessor : TodoProcessor
    {
        [IOCCBeanReference] private ListProvider listProvider;
        [IOCCBeanReference(Factory=typeof(DisplayFactory))] private ListDisplay listDisplay;
        public void Process()
        {
            TodoList list = listProvider.LoadList();
            listDisplay.DisplayList(list);
        }
    }
}