using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [Bean]
    public class ConsoleToDoProcessor : TodoProcessor
    {
        [BeanReference(Factory=typeof(ProviderFactory))] private ListProvider listProvider = null;
        [BeanReference(Factory=typeof(DisplayFactory))] private ListDisplay listDisplay = null;
        public void Process()
        {
            TodoList list = listProvider.LoadList();
            listDisplay.DisplayList(list);
        }
    }
}
