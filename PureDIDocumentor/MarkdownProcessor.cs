using PureDI;

namespace SimpleIOCCDocumentor
{
    [Bean]
    public class MarkdownProcessor : IMarkdownProcessor
    {
        public string ProcessFragment(string someMarkdown)
        {
            return Markdig.Markdown.ToHtml(someMarkdown);
        }
    }
}