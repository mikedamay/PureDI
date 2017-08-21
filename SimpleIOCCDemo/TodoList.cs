namespace SimpleIOCCDemo
{
    internal class TodoList
    {
        public TodoList(string contents)
        {
            this.Contents = contents;
        }

        public string Contents { get; }
    }
}