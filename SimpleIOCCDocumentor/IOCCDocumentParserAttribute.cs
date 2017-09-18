using System;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    public class IOCCDocumentParserAttribute : BeanReferenceBaseAttribute
    {
        [Bean]
        private class IOCCDocumentParserFactory : IFactory
        {
            [BeanReference] private SimpleIOCContainer simpleIocContainer;
            [BeanReference] private IPropertyMap propertyMap;
            [BeanReference(Name="navigator")] private XPathNavigatorResourceFactory navigatorFactory;
            public object Execute(BeanFactoryArgs args)
            {
                try
                {
                    object[] @params = (object[]) args.FactoryParmeter;
                    string documentPath = (string) @params[0];
                    string xmlRoot = (string) @params[1];
                    IOCCDocumentParser ddp = new IOCCDocumentParser(
                        (string)propertyMap.Map(documentPath)
                        ,xmlRoot, navigatorFactory);
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
}