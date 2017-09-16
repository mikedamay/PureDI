using System;
using System.Collections.Generic;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    [Bean]
    internal class UserGuideDocumentParser : IDocumentParser
    {
        [BeanReference(Name = "navigator", Factory = typeof(XPathNavigatorResourceFactory)
                , FactoryParameter = new object[] {typeof(SimpleIOCContainer)
                    , "SimpleIOCContainer.Docs.UserGuide.xml"})
        ]
        private XPathNavigator navigator = null;
        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = navigator.Select(
                $"/userGuide/group/topic[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
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
                "/userGuide/group/topic");
            while (nodes.MoveNext())
            {
                map.Add(new KeyValuePair<string, string>(
                    $"[{nodes.Current.InnerXml}](/UserGuide/{nodes.Current.InnerXml})"
                    , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }
}