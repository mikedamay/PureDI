using System;
using System.IO;
using System.Text;
using System.Xml.XPath;
using PureDI;
using PureDI.Attributes;
using static PureDI.Common.Common;

namespace SimpleIOCCDocumentor
{
    [Bean(Name="navigator")]
    public class XPathNavigatorResourceFactory : IFactory
    {
        [BeanReference] private IResourceProvider _resourceProvider = null;
        /// <param name="assemblyFinder">any type whose assembly is the assembly where the resource is stored</param>
        /// <param name="resourcePath">absolute path of resource, e.g. "DependencyInjector.IOCC.DiagnosticSchema.xml"
        ///   in case of doubt run ildasm against the assembly's binary and inspect the manifest
        ///   to ascertain the absolute path</param>
        /// <returns>an XPath navigator ready for calls to navigator.Select(xpath)</returns>
        public XPathNavigator ConvertResourceToXPathNavigator(Type assemblyFinder, string resourcePath)
        {
            string resourceString = _resourceProvider.GetResourceAsString(assemblyFinder, resourcePath);
            byte[] by = Encoding.UTF8.GetBytes(resourceString);
            using (Stream resourceStream = new MemoryStream(by))
            {
                XPathDocument xdoc = new XPathDocument(resourceStream);
                return xdoc.CreateNavigator();
            }
        }

        public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            object[] @params = (object[])args.FactoryParmeter;
            Assert(@params.Length == 2);
            Assert(@params[0] is Type);
            Assert(@params[1] is String);
            return (ConvertResourceToXPathNavigator(@params[0] as Type, @params[1] as String), injectionState);
        }
    }
}