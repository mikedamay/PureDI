using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

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
        private XPathNavigator navigator;
        private string resourcePath;
        private string xmlRoot;

        private XPathNavigatorResourceFactory factory;
        [BeanReference(Name = "navigator")]
        private XPathNavigatorResourceFactory Factory
        {
            set
            {
                XPathNavigatorResourceFactory factory = value;
                navigator = factory.ConvertResourceToXPathNavigator(typeof(SimpleIOCContainer), resourcePath);
            }
        }

        // inhibit IOCC diagnostic for no arg constructor
        public IOCCDocumentParser() { }

        /// <param name="resourcePath">either "SimpleIOCContainer.Docs.DiagnosticSchema.xml"}
        ///   or "SimpleIOCContainer.Docs.UserGuide.xml"} if it's anyhing else
        ///   then the resource needs to be in the SimpleIOCContainer assembly </param>
        /// <param name="xmlRoot">either "DiagnosticSchema" or "UserGuide" if anything else
        ///   then the document must be a superset of UserGuide.xml </param>
        public IOCCDocumentParser(string resourcePath, string xmlRoot)
        {
            this.xmlRoot = xmlRoot;
            this.resourcePath = resourcePath;
        }

        public string GetFragment(string fragmentType, string fragmentName)
        {
            XPathNodeIterator nodes = navigator.Select(
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
            XPathNodeIterator nodes = navigator.Select(
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
    public class IOCCDocumentParserAttribute : BeanReferenceBaseAttribute
    {
        [Bean]
        private class IOCCDocumentParserFactory : IFactory
        {
            [BeanReference] private SimpleIOCContainer simpleIocContainer;
            [BeanReference] private IPropertyMap propertyMap;
            public object Execute(BeanFactoryArgs args)
            {
                try
                {
                    object[] @params = (object[]) args.FactoryParmeter;
                    string documentPath = (string) @params[0];
                    string xmlRoot = (string) @params[1];
                    IOCCDocumentParser ddp = new IOCCDocumentParser(
                        (string)propertyMap.Map(documentPath)
                        ,xmlRoot);
                    simpleIocContainer.CreateAndInjectDependencies(ddp, out var diagnostics);
                    return ddp;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public IOCCDocumentParserAttribute(string DocumentPath = "", string XmlRoot = "")
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(DocumentPath));
            System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(XmlRoot));
            base.Factory = typeof(IOCCDocumentParserFactory);
            base.FactoryParameter = new object[] {DocumentPath, XmlRoot};
        }
    }

    internal interface IPropertyMap
    {
        object Map(string key);
        object Map(string key, object @default);
    }
    [Bean(Profile="noop")]
    public class NoopPropertyMap : IPropertyMap
    {
        public object Map(string key) => null;
        public object Map(string key, object @default) => @default;
    }

    [Bean]
    internal class Mapper : IPropertyMap
    {
        private readonly IDictionary<string, object> kvs;
        public Mapper()
        {
            kvs = new (string key, object value)[] {("", "")}.ToDictionary(p => p.key, p => p.value);
        }

        public object Map(string key)
        {
            if (kvs.ContainsKey(key))
            {
                return kvs[key];
            }
            return key;
        }
        public object Map(string key, object @default)
        {
            if (kvs.ContainsKey(key))
            {
                return kvs[key];
            }
            return @default;
        }
    }

    [Bean(Profile = "authoring")]
    internal class DocumentMapper : IPropertyMap
    {
        public object Map(string key)
        {
            const string root = "../";
            return Path.Combine(root, Path.ChangeExtension(key, null).Replace(".", "/")) + ".xml";
        }

        public object Map(string key, object @default) => @default;
    }
}