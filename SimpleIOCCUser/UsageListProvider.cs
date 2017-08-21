using System;
using System.Runtime.InteropServices.WindowsRuntime;
using com.TheDisappointedProgrammer.IOCC;
namespace SimpleIOCCDemo
{
    [IOCCBean]
    internal class UsageListProvider : ListProvider
    {
        public TodoList LoadList()
        {
           string contents = "There is no To Do List to display - the usage is dotnet run <filePath>";
           return new TodoList(contents);
        }
    }
}
