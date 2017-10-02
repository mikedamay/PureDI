using System;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    /// <summary>
    /// e.g. [IOCCDocumentPaarser(DocumentPath : "PDependencyInjector.Docs.UserGuide.xml", XmlRoot : "userGuide")]
    /// </summary>
    public class IOCCDocumentParserAttribute : BeanReferenceBaseAttribute
    {
        [Bean]
        private class IOCCDocumentParserFactory : IFactory
        {
            [BeanReference] private IPropertyMap propertyMap = null;
            [BeanReference(Name="navigator")] private XPathNavigatorResourceFactory navigatorFactory = null;
            public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
            {
                try
                {
                    object[] @params = (object[]) args.FactoryParmeter;
                    string documentPath = (string) @params[0];
                    string xmlRoot = (string) @params[1];
                    IOCCDocumentParser ddp = new IOCCDocumentParser(
                        (string)propertyMap.Map(documentPath)
                        ,xmlRoot, navigatorFactory);
                    return (ddp, injectionState);
                }
                catch (Exception ex)
                {
                    Exception dummy = ex;
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