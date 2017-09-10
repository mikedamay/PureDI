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
        [BeanReference] private MarkdownProcessor markdownProcessor;
        [BeanReference(Factory = typeof(ResourceFactory)
            , FactoryParameter = new object[] {typeof(ResourceFactory)
            , "SimpleIOCContainer.Docs.DiagnosticSchema.xml"})
          ]
        private string diagnosticSchema;
        private Stream diagnoStream;

        [BeanReference] private DocumentParser documentParser;

        //public DiagnosticProcessor([BeanReference(Factory=typeof(ResourceFactory)
        //  , FactoryParameter = new object[] {typeof(ResourceFactory)
        //  , "SimpleIOCContainer.Docs.DiagnosticSchema.xml"})] string diagnosticSchema
        //)
        //{ 
        //    byte[] by = Encoding.UTF8.GetBytes(diagnosticSchema);
        //    diagnoStream = new MemoryStream(by);            
        //}

        //~DiagnosticProcessor()
        //{
        //    if (diagnoStream != null)
        //    {
        //        diagnoStream.Dispose();
        //    }
        //}
        public string ProcessDiagnostic(string diagnosticName)
        {
            byte[] by = Encoding.UTF8.GetBytes(diagnosticSchema);
            diagnoStream = new MemoryStream(by);
            return documentParser.GetFragment(diagnoStream, "userGuide", diagnosticName);             
        }

    }
}