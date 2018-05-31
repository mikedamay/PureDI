using System;
using System.Reflection;
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
        Type ResourceAssemblyFinder { set; }
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
        private Type _resourceAssemblyFinder;
        public Type ResourceAssemblyFinder
        {
            set { _resourceAssemblyFinder = value; QueryMakeNavigator(); }
        }

        private void QueryMakeNavigator()
        {
            if (_factory != null && _resourcePath != null && _resourceAssemblyFinder != null)
            {
                _navigator = _factory.ConvertResourceToXPathNavigator(_resourceAssemblyFinder, _resourcePath);
            }
        }
    }
    [Bean(Profile = "authoring")]
    internal class XPathNavigatorNoCache : IIOCCXPathNavigatorCache
    {
        public XPathNavigator Navigator => MakeNavigator();
        private XPathNavigatorResourceFactory _factory;
        public XPathNavigatorResourceFactory Factory { set { _factory = value; } }
        private string _resourcePath;
        public string ResourcePath { set { _resourcePath = value; } }
        private Type _resourceAssemblyFinder;
        public Type ResourceAssemblyFinder
        {
            set { _resourceAssemblyFinder = value; }
        }

        private XPathNavigator MakeNavigator()
        {
            return _factory.ConvertResourceToXPathNavigator(_resourceAssemblyFinder, _resourcePath);
        }
    }
}
