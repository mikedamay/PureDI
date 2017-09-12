using System;
using System.IO;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;

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
            //XPathDocument xdoc = new XPathDocument(diagnosticStream);
            //XPathNavigator navigator = xdoc.CreateNavigator();
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
    }
}