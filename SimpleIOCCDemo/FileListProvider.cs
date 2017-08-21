using System;
using System.Runtime.InteropServices.WindowsRuntime;
using com.TheDisappointedProgrammer.IOCC;
namespace SimpleIOCCDemo
{
    [IOCCBean]
    internal class FileListProvider : ListProvider
    {
        private string location;
        [IOCCConstructor]
        public FileListProvider(
          [IOCCBeanReference(Factory=typeof(CommandLineFactory))] string location)
        {
            this.location = location 
              ?? throw new Exception(
              "A location for the todo list was expected.  You must enter a file location on the command line");
        }
        public TodoList LoadList()
        {
           string contents = System.IO.File.ReadAllText(location);
            return new TodoList(contents);
        }
    }
}