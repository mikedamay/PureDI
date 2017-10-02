using System.Reflection.Metadata;
using PureDI;
using System.IO;
using PureDI.Common;
using IOPath = System.IO.Path; 

namespace SimpleIOCCDocumentor
{
    public interface IDocumentationSiteGenerator
    {
        void GenerateSite();
    }
    [Bean]
    public class DocumentationSiteGenerator : IDocumentationSiteGenerator
    {
        // very wasteful to instantiate the parsers 2x.
        // we really need constructor paraemters
        [IOCCDocumentParser(
            DocumentPath: "PureDI.Docs.UserGuide.xml"
            , XmlRoot: Constants.USER_GUIDE_ROOT)]
        private IDocumentParser userGuideDocumentParser = null;
        [IOCCDocumentParser(
            DocumentPath: "PureDI.Docs.DiagnosticSchema.xml"
            , XmlRoot: Constants.DIAGNOSTIC_SCHEMA_ROOT)]
        private IDocumentParser diagnosticSchemaDocumentParser = null;

        private string path = null;

        [Config(Key: "relativePath")]
        private string Path
        {
            set { path = IOPath.Combine(
              IOPath.GetDirectoryName(
              this.GetType().Assembly.Location), value); }
        }

        [BeanReference] private IDocumentProcessor documentProcessor = null;

        public void GenerateSite()
        {
            GenerateIndex();
            GenerateFilesFromDocument("UserGuide", userGuideDocumentParser);
            GenerateFilesFromDocument("DiagnosticSchema", diagnosticSchemaDocumentParser);
        }

        private void GenerateIndex()
        {
            var index = documentProcessor.ProcessDocument(string.Empty, string.Empty);
            File.WriteAllText(IOPath.Combine(path, "index.html"), index);
        }

        private void GenerateFilesFromDocument(string documentName, IDocumentParser documentParser)
        {
            var index = documentParser.GetDocumentIndex().Keys;
            foreach (var fragmentKey in index)
            {
                string fragmentName = Cleanup(fragmentKey);
                string fragment = documentProcessor.ProcessDocument(documentName, fragmentName);
                SaveAsDocument(documentName, fragmentName, fragment);
            }
        }
        /// <param name="fragmentKey">e.g."[Introduction](/Simple/UserGuide/Instroduction)"</param>
        /// <returns>"Introduction"</returns>
        private string Cleanup(string fragmentKey)
        {
            var parts = fragmentKey.Split("]");
            return parts[0].Replace("[", string.Empty);
                    // TODO we should be using the router to do this
        }

        /// <param name="documentName">e.g. UserGuide or DiagnosticSchema - no extension</param>
        /// <param name="fragment">a string of html which will sit quietly until rendered</param>
        private void SaveAsDocument(string documentName, string fragmentName, string fragment)
        {
            File.WriteAllText(IOPath.Combine(IOPath.Combine(path, documentName), fragmentName) + ".html", fragment);
        }
    }
}