using System;
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
        DependencyInjector pdi = new DependencyInjector(profiles: new string[] {"authoring"});
        [BeanReference] private IPropertyMap propertyMap = null;        
        [BeanReference(Name = "navigator")] private XPathNavigatorResourceFactory navigatorFactory = null;
        public (IDocumentationSiteGenerator, IDocumentProcessor, InjectionState) 
          Configure(InjectionState injectionState = null)
        {
            injectionState = pdi.CreateAndInjectDependencies(this, injectionState: injectionState
                , deferDepedencyInjection: false).injectionState;
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"site-userguide", "PureDIDocumentor.Docs.UserGuide.xml", Constants.UserGuideRoot
              ,typeof(DependencyConfiguration));
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"site-diagnostics", "PureDI.Docs.DiagnosticSchema.xml", Constants.DiagnosticSchemaRoot
              ,typeof(DependencyInjector));
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"doc-userguide", "PureDIDocumentor.Docs.UserGuide.xml", Constants.UserGuideRoot
              ,typeof(DependencyConfiguration));
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
          ,string beanName, string documentPath, string xmlRoot, Type resourceAssemblyFinder)
        {
            IIOCCXPathNavigatorCache nc;
            (nc, injectionState) = pdi.CreateAndInjectDependencies<IIOCCXPathNavigatorCache>(injectionState);
            nc.Factory = navigatorFactory;
            nc.ResourcePath = (string) propertyMap.Map(documentPath);
            nc.ResourceAssemblyFinder = resourceAssemblyFinder;
            IOCCDocumentParser ddp;
            (ddp, injectionState) = pdi.CreateAndInjectDependencies<IOCCDocumentParser>(injectionState);            
            ddp.XmlRoot = xmlRoot;
            ddp.NavigatorCache = nc;
            return pdi.CreateAndInjectDependencies(ddp, injectionState: injectionState, rootBeanSpec:
                new RootBeanSpec(rootBeanName: beanName), deferDepedencyInjection: false).injectionState;
        }
    }
}