using System;
using System.IO;
using System.Linq;
using System.Text;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    public interface IDocumentProcessor
    {
        string ProcessDocument(string document, string fragment);
    }
    [Bean]
    public class DocumentProcessor : IDocumentProcessor
    {
        [BeanReference] private MarkdownProcessor markdownProcessor = null;
        [BeanReference] private DiagnosticDocumentParser diagnosticDocumentParser = null;
        [BeanReference] private UserGuideDocumentParser userGuideDocumentParser = null;
        [BeanReference(Factory = typeof(ResourceFactory)
                , FactoryParameter = new object[] {typeof(Program)
                    , "SimpleIOCCDocumentor.wwwroot.docwrapper.html"})
        ]
        private string htmlWrapper = null;

        public string ProcessDocument(string document, string fragment)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                var diagnosticIndex = diagnosticDocumentParser.GetDocumentIndex();
                var userGuideIndex = userGuideDocumentParser.GetDocumentIndex();
                string str = markdownProcessor.ProcessFragment(string.Join(Environment.NewLine + Environment.NewLine
                  , diagnosticIndex.Keys.Union(userGuideIndex.Keys).OrderBy(k => k)));
                return str;
            }
            else
            {
                IDocumentParser parser = string.Compare(document, "diagnosticSchema",
                    StringComparison.InvariantCultureIgnoreCase) == 0
                        ? diagnosticDocumentParser as IDocumentParser
                        : userGuideDocumentParser;
                var titleMD = parser.GetFragment("userGuideTitle", fragment);
                var titleHtml = markdownProcessor.ProcessFragment(titleMD);
                var userGuideMD = parser.GetFragment("userGuide", fragment);
                var userGuideHtml = markdownProcessor.ProcessFragment(userGuideMD);
                var wrapper = htmlWrapper.Replace("{userGuideTitle}", titleHtml).Replace("{userGuide}", userGuideHtml);
                return wrapper;
            }
        }

    }
}