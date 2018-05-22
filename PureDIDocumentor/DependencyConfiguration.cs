using SimpleIOCCDocumentor;
using PureDI;

namespace PureDIDocumentor
{
    internal class DependencyConfiguration
    {
        public (IDocumentationSiteGenerator, IDocumentProcessor, InjectionState) Configure()
        {
            PDependencyInjector pdi = new PDependencyInjector(profiles: new string[] {"authoring"});
            InjectionState @is;
            (_, @is) =
                pdi.CreateAndInjectDependencies(new GenericConfig(("relativePath", "../../../../Simple")))
                ;
            (IDocumentProcessor dp, InjectionState is2) = pdi.CreateAndInjectDependencies<
                IDocumentProcessor>(@is);
            (var dsg, var is3) = pdi.CreateAndInjectDependencies<IDocumentationSiteGenerator>(is2);
            return (dsg, dp, is3);
        }
    }
}