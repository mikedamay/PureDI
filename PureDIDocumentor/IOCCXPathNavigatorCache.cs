using System.Xml.XPath;
using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDocumentor
{
    internal interface IIOCCXPathNavigatorCache
    {
        XPathNavigator Navigator { get; }
        XPathNavigatorResourceFactory Factory { set; }
        string ResourcePath { set; }
    }
    [Bean]
    internal class XPathNavigatorFixedCache : IIOCCXPathNavigatorCache
    {
        private XPathNavigator _navigator;
        public XPathNavigator Navigator => _navigator;
        private XPathNavigatorResourceFactory _factory;
        public XPathNavigatorResourceFactory Factory { set { _factory = value; QueryMakeNavigator(); } }
        private string _resourcePath;
        public string ResourcePath { set { _resourcePath = value; QueryMakeNavigator(); } }

        private void QueryMakeNavigator()
        {
            if (_factory != null && _resourcePath != null)
            {
                _navigator = _factory.ConvertResourceToXPathNavigator(typeof(PDependencyInjector), _resourcePath);
            }
        }
    }
    [Bean(Profile = "authoring")]
    internal class XPathNavigatorNoCache : IIOCCXPathNavigatorCache
    {
        private XPathNavigator _navigator = null;
        public XPathNavigator Navigator => MakeNavigator();
        private XPathNavigatorResourceFactory _factory;
        public XPathNavigatorResourceFactory Factory { set { _factory = value; } }
        private string _resourcePath;
        public string ResourcePath { set { _resourcePath = value; } }

        private XPathNavigator MakeNavigator()
        {
            _ = _navigator;
            return _factory.ConvertResourceToXPathNavigator(typeof(PDependencyInjector), _resourcePath);
        }
    }
}
