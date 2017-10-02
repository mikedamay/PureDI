using System;
using System.Collections.Generic;
using System.Text;
using PureDI;

namespace SimpleIOCCDemo
{
    [Bean]
    internal class ConsoleDisplay : ListDisplay
    {
        public void DisplayList(TodoList todoList)
        {
            Console.WriteLine(todoList.Contents);
        }
    }
}
