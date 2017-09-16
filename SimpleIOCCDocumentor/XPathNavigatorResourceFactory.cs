using System;
using System.IO;
using System.Text;
using System.Xml.XPath;
using com.TheDisappointedProgrammer.IOCC;
using static com.TheDisappointedProgrammer.IOCC.Common;

namespace SimpleIOCCDocumentor
{
    [Bean(Name="navigator")]
    public class XPathNavigatorResourceFactory : ResourceFactoryBase
    {
        public XPathNavigator ConvertResourceToXPathNavigator(Type assemblyFinder, string resourcePath)
        {
            string diagnosticSchema = GetResourceAsString(assemblyFinder, resourcePath);
            byte[] by = Encoding.UTF8.GetBytes(diagnosticSchema);
            using (Stream diagnosticStream = new MemoryStream(by))
            {
                XPathDocument xdoc = new XPathDocument(diagnosticStream);
                return xdoc.CreateNavigator();
            }
        }

        public override object Execute(BeanFactoryArgs args)
        {
            object[] @params = (object[])args.FactoryParmeter;
            Assert(@params.Length == 2);
            Assert(@params[0] is Type);
            Assert(@params[1] is String);
            return ConvertResourceToXPathNavigator(@params[0] as Type, @params[1] as String);
        }
    }
}