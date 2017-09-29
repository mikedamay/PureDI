using System;
using System.IO;
using System.Text;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;
using static com.TheDisappointedProgrammer.IOCC.Common.Common;

namespace SimpleIOCCDocumentor
{
    [Bean(Name="navigator")]
    public class XPathNavigatorResourceFactory : IFactory
    {
        [BeanReference] private IDocumentMaker documentMaker = null;
        /// <param name="assemblyFinder">any type whose assembly matches that where the resource is stored</param>
        /// <param name="resourcePath">absolute path of resource, e.g. "SimpleIOCContainer.IOCC.DiagnosticSchema.xml"
        ///   in case of doubt run ildasm against the assembly's binary and inspect the manifest
        ///   to ascertain the absolute path</param>
        /// <returns>an XPath navigator ready for calls to navigator.Select(xpath)</returns>
        public XPathNavigator ConvertResourceToXPathNavigator(Type assemblyFinder, string resourcePath)
        {
            string diagnosticSchema = documentMaker.GetResourceAsString(assemblyFinder, resourcePath);
            byte[] by = Encoding.UTF8.GetBytes(diagnosticSchema);
            using (Stream diagnosticStream = new MemoryStream(by))
            {
                XPathDocument xdoc = new XPathDocument(diagnosticStream);
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