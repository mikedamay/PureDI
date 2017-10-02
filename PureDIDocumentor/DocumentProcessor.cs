using System;
using System.Linq;
using PureDI;
using PureDI.Common;

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
        [IOCCDocumentParser(
            DocumentPath: "PureDI.Docs.UserGuide.xml"
            , XmlRoot: Constants.USER_GUIDE_ROOT)]
        private IDocumentParser userGuideDocumentParser = null;
        [BeanReference(Factory = typeof(ResourceFactory)
                , FactoryParameter = new object[] {typeof(Program)
                    , "PureDIDocumentor.wwwroot.docwrapper.html"})
        ]
        private string htmlWrapper = null;

        public string ProcessDocument(string document, string fragment)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                var userGuideIndex = userGuideDocumentParser.GetDocumentIndex();
                string str = markdownProcessor.ProcessFragment(string.Join(Environment.NewLine + Environment.NewLine
                  , userGuideIndex.Keys.OrderBy(k => k)));
                return str;
            }
            else if (string.Compare(document, "userGuide", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return "";
            }
            else
            {
                IDocumentParser parser = userGuideDocumentParser;
                var titleMD = parser.GetFragment("userGuideTitle", fragment);
                var titleHtml = titleMD; // markdownProcessor.ProcessFragment(titleMD);
                var userGuideMD = parser.GetFragment("userGuide", fragment);
                var userGuideHtml = markdownProcessor.ProcessFragment(userGuideMD);
                var wrapper = htmlWrapper.Replace("{userGuideTitle}", titleHtml).Replace("{userGuide}", userGuideHtml);
                return wrapper;
            }
        }

    }
}