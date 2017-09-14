using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIOCCDocumentor
{
    [Bean]
    internal class DocumentParser
    {
        [BeanReference(Name="navigator", Factory = typeof(XPathNavigatorResourceFactory)
                , FactoryParameter = new object[] {typeof(SimpleIOCContainer)
                    , "SimpleIOCContainer.Docs.DiagnosticSchema.xml"})
        ]
        private XPathNavigator navigator;
        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = navigator.Select(
              $"/diagnosticSchema/group/causeCode[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
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
            IDictionary<string, string> map = new ConcurrentDictionary<string, string>();
            XPathNodeIterator nodes = navigator.Select(
                "/diagnosticSchema/group/causeCode");
            while (nodes.MoveNext())
            {
                map.Add(new KeyValuePair<string, string>(
                  $"[{nodes.Current.InnerXml}](http://localhost:60653/{nodes.Current.InnerXml})"
                  , $"({nodes.Current.InnerXml})"));
            }
            return map;
        }
    }
}