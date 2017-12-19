using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using PureDI;

namespace SimpleIOCCDocumentor
{
    [Ignore]
    internal interface IDocumentParser
    {
        string GetFragment(string fragmentType, string fragmentName);
        IDictionary<string, string> GetDocumentIndex();
    }
    [Bean]
    internal class IOCCDocumentParser : IDocumentParser
    {
        private string resourcePath;
        private string xmlRoot;
        [BeanReference] private ICodeFetcher codeFetcher = null;
        private IIOCCXPathNavigatorCache navigatorCache;
        [BeanReference(Scope=BeanScope.Prototype)]
        private IIOCCXPathNavigatorCache NavigatorCache
        {
            set
            {
                navigatorCache = value;
                navigatorCache.Factory = factory;
                navigatorCache.ResourcePath = resourcePath;
            }
        }

        private readonly XPathNavigatorResourceFactory factory;

        // inhibit PureDI diagnostic for no arg constructor
        public IOCCDocumentParser() { }
        /// <param name="resourcePath">either "PDependencyInjector.Docs.DiagnosticSchema.xml"}
        ///   or "PDependencyInjector.Docs.UserGuide.xml"} if it's anyhing else
        ///   then the resource needs to be in the PDependencyInjector assembly </param>
        /// <param name="xmlRoot">either "DiagnosticSchema" or "UserGuide" if anything else
        ///   then the document must be a superset of UserGuide.xml </param>
        /// <param name="factory">provides tha pparopriate resources e.g. user guide, diagnostics etc.</param>
        public IOCCDocumentParser(string resourcePath, string xmlRoot, XPathNavigatorResourceFactory factory)
        {
            this.xmlRoot = xmlRoot;
            this.resourcePath = resourcePath;
            this.factory = factory;
        }

        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = navigatorCache.Navigator.Select(
                $"/{xmlRoot}/group/topic[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
            if (nodes.MoveNext())
            {
                return codeFetcher.SubstituteCode(nodes.Current.InnerXml);
            }
            else
            {
                throw new Exception($"fragment type: {fragmentType}, fragment name: {fragmentName}");
            }
        }

        public IDictionary<string, string> GetDocumentIndex()
        {
            IDictionary<string, string> map = new Dictionary<string, string>();
            XPathNodeIterator nodes = navigatorCache.Navigator.Select(
                $"/{xmlRoot}/group/topic");
            while (nodes.MoveNext())
            {
                map.Add(new KeyValuePair<string, string>(
                    $"[{nodes.Current.InnerXml}](/Simple/{xmlRoot}/{nodes.Current.InnerXml}.html)"
                    // e.g. "[MissingBean](/diagnosticSchema/MissingBean)"
                    , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }

}