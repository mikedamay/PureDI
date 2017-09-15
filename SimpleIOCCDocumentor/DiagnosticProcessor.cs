using System;
using System.IO;
using System.Text;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    public interface IDiagnosticProcessor
    {
        string ProcessDiagnostic(string diagnosticName);
    }
    [Bean]
    public class DiagnosticProcessor : IDiagnosticProcessor
    {
        [BeanReference] private MarkdownProcessor markdownProcessor = null;
        [BeanReference] private DocumentParser documentParser = null;
        [BeanReference(Factory = typeof(ResourceFactory)
                , FactoryParameter = new object[] {typeof(Program)
                    , "SimpleIOCCDocumentor.wwwroot.docwrapper.html"})
        ]
        private string htmlWrapper = null;

        public string ProcessDiagnostic(string diagnosticName)
        {
            if (string.IsNullOrWhiteSpace(diagnosticName))
            {
                var index = documentParser.GetDocumentIndex();
                string str = markdownProcessor.ProcessFragment(string.Join(Environment.NewLine + Environment.NewLine, index.Keys));
                return str;
            }
            else
            {
                var titleMD = documentParser.GetFragment("userGuideTitle", diagnosticName);
                var titleHtml = markdownProcessor.ProcessFragment(titleMD);
                var userGuideMD = documentParser.GetFragment("userGuide", diagnosticName);
                var userGuideHtml = markdownProcessor.ProcessFragment(userGuideMD);
                var wrapper = htmlWrapper.Replace("{userGuideTitle}", titleHtml).Replace("{userGuide}", userGuideHtml);
                return wrapper;
            }
        }

    }
}