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
/*
            injectionState = pdi.CreateAndInjectDependencies(this, injectionState: injectionState
                , deferDepedencyInjection: false).injectionState;
*/
/*
            injectionState = CreateAndInjectDocumentParser(injectionState
              ,"site-userguide", "PureDI.Docs.UserGuide.xml", Constants.UserGuideRoot);
*/
            
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
            IOCCDocumentParser ddp = new IOCCDocumentParser(
                (string)propertyMap.Map(documentPath)
                ,xmlRoot, navigatorFactory);
            return pdi.CreateAndInjectDependencies(ddp, injectionState: injectionState, rootBeanSpec:
                new RootBeanSpec(rootBeanName: beanName), deferDepedencyInjection: true).injectionState;

        }
    }
}