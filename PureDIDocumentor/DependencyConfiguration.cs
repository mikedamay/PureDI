using Microsoft.Extensions.ObjectPool;
using SimpleIOCCDocumentor;
using PureDI;
using PureDI.Attributes;
using PureDI.Common;

namespace PureDIDocumentor
{
    [Bean]
    internal class DependencyConfiguration
    {
        PDependencyInjector pdi = new PDependencyInjector(profiles: new string[] {"authoring"});
        [BeanReference] private IPropertyMap propertyMap = null;        
        [BeanReference(Name = "navigator")] private XPathNavigatorResourceFactory navigatorFactory = null;
        public (IDocumentationSiteGenerator, IDocumentProcessor, InjectionState) 
          Configure(InjectionState injectionState = null)
        {
            injectionState = pdi.CreateAndInjectDependencies(this, injectionState: injectionState
                , deferDepedencyInjection: false).injectionState;
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"site-userguide", "PureDI.Docs.UserGuide.xml", Constants.UserGuideRoot);
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"site-diagnostics", "PureDI.Docs.DiagnosticSchema.xml", Constants.DiagnosticSchemaRoot);
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"doc-userguide", "PureDI.Docs.UserGuide.xml", Constants.UserGuideRoot);
            InjectionState @is;
            (_, @is) =
                pdi.CreateAndInjectDependencies(
                new GenericConfig(("relativePath", "../../../../Simple")), injectionState: injectionState)
                ;
            (IDocumentProcessor dp, InjectionState is2) = pdi.CreateAndInjectDependencies<
                IDocumentProcessor>(@is);
            (var dsg, var is3) = pdi.CreateAndInjectDependencies<IDocumentationSiteGenerator>(is2);
            return (dsg, dp, is3);
        }

        private InjectionState CreateAndInjectDocumentParser(InjectionState injectionState
          ,string beanName, string documentPath, string xmlRoot)
        {
            IIOCCXPathNavigatorCache nc;
            (nc, injectionState) = pdi.CreateAndInjectDependencies<IIOCCXPathNavigatorCache>(injectionState);
            nc.Factory = navigatorFactory;
            nc.ResourcePath = (string) propertyMap.Map(documentPath);
            IOCCDocumentParser ddp = new IOCCDocumentParser(
                xmlRoot, nc);
/*
            IDocumentParser ddp;
            (ddp, injectionState) = pdi.CreateAndInjectDependencies<IDocumentParser>(injectionState);
            ddp.XmlRoot = xmlRoot;
            ddp.NavigatorCache = nc;
*/
            return pdi.CreateAndInjectDependencies(ddp, injectionState: injectionState, rootBeanSpec:
                new RootBeanSpec(rootBeanName: beanName), deferDepedencyInjection: true).injectionState;

        }
    }
}