using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    [IOCCIgnore]
    internal interface IDocumentParser
    {
        string GetFragment(string fragmentType, string fragmentName);
        IDictionary<string, string> GetDocumentIndex();
    }
    [Bean]
    internal class IOCCDocumentParser : IDocumentParser
    {
        //private XPathNavigator navigator;
        private string resourcePath;
        private string xmlRoot;

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

        // inhibit IOCC diagnostic for no arg constructor
        public IOCCDocumentParser() { }

        /// <param name="resourcePath">either "SimpleIOCContainer.Docs.DiagnosticSchema.xml"}
        ///   or "SimpleIOCContainer.Docs.UserGuide.xml"} if it's anyhing else
        ///   then the resource needs to be in the SimpleIOCContainer assembly </param>
        /// <param name="xmlRoot">either "DiagnosticSchema" or "UserGuide" if anything else
        ///   then the document must be a superset of UserGuide.xml </param>
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
                return nodes.Current.InnerXml;
            }
            else
            {
                throw new Exception();
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
                    $"[{nodes.Current.InnerXml}](/{xmlRoot}/{nodes.Current.InnerXml})"
                    // e.g. "[MissingBean](/diagnosticSchema/MissingBean)"
                    , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }
    // e.g. [IOCCDocumentPaarser(DocumentPath : "xxx.yyy.zzz.xml", XmlRoot : "myroot")]

}