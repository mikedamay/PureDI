using System.Collections.Generic;
using System.IO;
using PureDI;
using Microsoft.AspNetCore.Hosting.Builder;
using PureDI.Attributes;

namespace SimpleIOCCDocumentor
{
    /// <summary>
    /// the IAPIDocumntParser takes an assmebly 
    /// </summary>
    public interface IApiDocumentParser
    {
        string GetFragment(string fragmentName);
        IDictionary<string, string> GetDocumentIndex();
    }

    [Bean]
    public class ApiDocParser : IApiDocumentParser
    {
        /// <summary>
        /// dictionary of fragment name to html string representing page
        /// </summary>
        private IDictionary<string, string> fragments = null;

        [BeanReference(Factory = typeof(ResourceFactory)
            , FactoryParameter = new object[] {typeof(PDependencyInjector), "PureDI.Docs.apidoc.xsl"})]
        private string apiDocStyleSheet = null;
        public ApiDocParser()
        {
            var x = fragments;
            var y = apiDocStyleSheet;
            string apiDocPath = Path.Combine(
              Path.GetDirectoryName(
              this.GetType().Assembly.Location)
              , "../../../../PureDI/bin/Debug/PDependencyInjector.xml");

        }
        public string GetFragment(string fragmentName)
        {
            
            return string.Empty;
        }

        public IDictionary<string, string> GetDocumentIndex()
        {
            throw new System.NotImplementedException();
        }
    }
}