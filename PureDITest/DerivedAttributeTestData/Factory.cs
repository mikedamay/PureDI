using System;
using System.Dynamic;
using System.IO;
using System.Reflection;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    [Ignore]
    public abstract class ResourceFactoryBase : IFactory
    {
        public string GetResourceAsString(Type assemblyFinder, string resourcePath)
        {
            using (Stream s = assemblyFinder.Assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader sr = new StreamReader(s))
            {
                return sr.ReadToEnd();
            }
        }

        public virtual (object bean, InjectionState injectionState)
            Execute(InjectionState injectionState, BeanFactoryArgs args)
        {
            object[] @params = (object[]) args.FactoryParmeter;
            //Assert(@params.Length == 2);
            //Assert(@params[0] is Type);
            //Assert(@params[1] is String);
            return (GetResourceAsString(@params[0] as Type, @params[1] as String), injectionState);

        }
    }

    public class ResourceAttribute : BeanReferenceBaseAttribute
    {
        [Bean]
        private class ResourceFactory : ResourceFactoryBase {}

        public ResourceAttribute(Type assemblyFinder = null, string resourcePath = null)
        {
            base.Name = this.Name;
            base.ConstructorName = this.ConstructorName;
            base.Factory = typeof(ResourceFactory);
            base.FactoryParameter = new object[] {assemblyFinder, resourcePath};
        }
    }


    [Bean]
    public class Factory : IResultGetter
    {
        [Resource(typeof(PDependencyInjector)
          , "PureDI.Docs.DiagnosticSchema.xml")]
        public string resource;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Resource = resource;
            return eo;
        }
    }
}