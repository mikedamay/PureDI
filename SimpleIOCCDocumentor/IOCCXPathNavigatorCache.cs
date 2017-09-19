using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    internal interface IIOCCXPathNavigatorCache
    {
        XPathNavigator Navigator { get; }
        XPathNavigatorResourceFactory Factory { set; }
        string ResourcePath { set; }
    }
    [Bean]
    internal class XPathNavigatoorFixedCache : IIOCCXPathNavigatorCache
    {
        public XPathNavigator navigator;
        public XPathNavigator Navigator => navigator;
        private XPathNavigatorResourceFactory factory;
        public XPathNavigatorResourceFactory Factory { set { factory = value; QueryMakeNavigator(); } }
        private string resourcePath;
        public string ResourcePath { set { resourcePath = value; QueryMakeNavigator(); } }

        private void QueryMakeNavigator()
        {
            if (factory != null && resourcePath != null)
            {
                navigator = factory.ConvertResourceToXPathNavigator(typeof(SimpleIOCContainer), resourcePath);
            }
        }
    }
    [Bean(Profile = "authoring")]
    internal class XPathNavigatorNoCache : IIOCCXPathNavigatorCache
    {
        public XPathNavigator navigator = null;
        public XPathNavigator Navigator => MakeNavigator();
        private XPathNavigatorResourceFactory factory;
        public XPathNavigatorResourceFactory Factory { set { factory = value; } }
        private string resourcePath;
        public string ResourcePath { set { resourcePath = value; } }

        private XPathNavigator MakeNavigator()
        {
            return factory.ConvertResourceToXPathNavigator(typeof(SimpleIOCContainer), resourcePath);
        }
    }
}
