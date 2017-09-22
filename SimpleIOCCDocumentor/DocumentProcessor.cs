using System;
using System.Linq;
using com.TheDisappointedProgrammer.IOCC;
using com.TheDisappointedProgrammer.IOCC.Common;

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
            DocumentPath: "SimpleIOCContainer.Docs.UserGuide.xml"
            , XmlRoot: Constants.USER_GUIDE_ROOT)]
        private IOCCDocumentParser userGuideDocumentParser = null;
        [IOCCDocumentParser(
            DocumentPath: "SimpleIOCContainer.Docs.DiagnosticSchema.xml"
            , XmlRoot: Constants.DIAGNOSTIC_SCHEMA_ROOT)]
        private IOCCDocumentParser diagnosticSchemaDocumentParser = null;
        [BeanReference(Factory = typeof(ResourceFactory)
                , FactoryParameter = new object[] {typeof(Program)
                    , "SimpleIOCCDocumentor.wwwroot.docwrapper.html"})
        ]
        private string htmlWrapper = null;

        public string ProcessDocument(string document, string fragment)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                var diagnosticIndex = diagnosticSchemaDocumentParser.GetDocumentIndex();
                var userGuideIndex = userGuideDocumentParser.GetDocumentIndex();
                string str = markdownProcessor.ProcessFragment(string.Join(Environment.NewLine + Environment.NewLine
                  , diagnosticIndex.Keys.Union(userGuideIndex.Keys).OrderBy(k => k)));
                return str;
            }
            else
            {
                IDocumentParser parser = string.Compare(document, "diagnosticSchema",
                    StringComparison.InvariantCultureIgnoreCase) == 0
                        ? diagnosticSchemaDocumentParser as IDocumentParser
                        : userGuideDocumentParser;
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