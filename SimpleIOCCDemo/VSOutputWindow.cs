using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [Bean(Name="outputWindow")]
    internal class VSOutputWindow : ListDisplay
    {
        public void DisplayList(TodoList todoList)
        {
             System.Diagnostics.Debug.WriteLine(todoList.Contents);
        }
    }
}