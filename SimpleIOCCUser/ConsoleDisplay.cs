using System;
using System.Collections.Generic;
using System.Text;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
    [IOCCBean]
    internal class ConsoleDisplay : ListDisplay
    {
        public void DisplayList(TodoList todoList)
        {
            Console.WriteLine(todoList.Contents);
        }
    }
}
