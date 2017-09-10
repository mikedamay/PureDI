using System;
using System.IO;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    [Bean]
    internal class DocumentParser
    {
        public string GetFragment(Stream diagnosticStream, string fragmentType, string fragmentName)
        {
            XPathDocument xdoc = new XPathDocument(diagnosticStream);
            XPathNavigator nav = xdoc.CreateNavigator();
            XPathNodeIterator nodes = nav.Select($"/diagnosticSchema/group/causeCode[text() = \'{fragmentName}\']/following-sibling::{fragmentType}");
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