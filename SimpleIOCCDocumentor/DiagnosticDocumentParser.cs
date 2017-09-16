using System;
using System.Collections.Generic;
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
    internal class DiagnosticDocumentParser : IDocumentParser
    {
        [BeanReference(Name = "navigator", Factory = typeof(XPathNavigatorResourceFactory)
                , FactoryParameter = new object[] {typeof(SimpleIOCContainer)
                    , "SimpleIOCContainer.Docs.DiagnosticSchema.xml"})
        ]
        private XPathNavigator navigator = null;
        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = navigator.Select(
                $"/diagnosticSchema/group/topic[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
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
            XPathNodeIterator nodes = navigator.Select(
                "/diagnosticSchema/group/topic");
            while (nodes.MoveNext())
            {
                map.Add(new KeyValuePair<string, string>(
                    $"[{nodes.Current.InnerXml}](/diagnosticSchema/{nodes.Current.InnerXml})"
                    , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }
}