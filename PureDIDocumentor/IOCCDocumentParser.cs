using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDocumentor
{
    internal interface IDocumentParser
    {
        string GetFragment(string fragmentType, string fragmentName);
        IDictionary<string, string> GetDocumentIndex();
        string XmlRoot { get; set; }
        IIOCCXPathNavigatorCache NavigatorCache { get; set; }
    }
    [Bean]
    internal class IOCCDocumentParser : IDocumentParser
    {
        private string _xmlRoot;
        private IIOCCXPathNavigatorCache _navigatorCache;

        public string XmlRoot
        {
            get => _xmlRoot;
            set => _xmlRoot = value;
        }

        public IIOCCXPathNavigatorCache NavigatorCache
        {
            get => _navigatorCache;
            set => _navigatorCache = value;
        }

        [BeanReference] private ICodeFetcher codeFetcher = null;
        // inhibit PureDI diagnostic for no arg constructor
        public IOCCDocumentParser() { }

        /// <param name="xmlRoot">either "DiagnosticSchema" or "UserGuide" if anything else
        ///     then the document must be a superset of UserGuide.xml </param>
        /// <param name="navigatorCache"></param>
        public IOCCDocumentParser(string xmlRoot, IIOCCXPathNavigatorCache navigatorCache)
        {
            this._xmlRoot = xmlRoot;
            this._navigatorCache = navigatorCache;
        }

        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = _navigatorCache.Navigator.Select(
                $"/{_xmlRoot}/group/topic[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
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
            XPathNodeIterator nodes = _navigatorCache.Navigator.Select(
                $"/{_xmlRoot}/group/topic");
            while (nodes.MoveNext())
            {
                map.Add(new KeyValuePair<string, string>(
                    $"[{nodes.Current.InnerXml}](/Simple/{_xmlRoot}/{nodes.Current.InnerXml}.html)"
                    // e.g. "[MissingBean](/diagnosticSchema/MissingBean)"
                    , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }

}