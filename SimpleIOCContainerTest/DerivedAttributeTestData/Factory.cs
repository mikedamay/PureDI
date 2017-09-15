using System;
using System.Dynamic;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    public class ResourceAttribute : BeanReferenceAttribute
    {
        public Type AssemblyFinder = null;
        public string ResourcePath = null;
        private class ResourceFactory : ResourceFactoryBase {}
        ResourceFactory resourceFactory = new ResourceFactory();
        public ResourceAttribute(Type assemblyFinder, string resourcePath)
        {
            this.AssemblyFinder = assemblyFinder;
            this.ResourcePath = resourcePath;
            base.Name = this.Name;
            base.ConstructorName = this.ConstructorName;
            base.Factory = this.Factory;
        }

        public object Execute(BeanFactoryArgs args)
        {
            return resourceFactory.Execute(args);
        }
    }


    [Bean]
    public class Factory : IResultGetter
    {
        [Resource(typeof(SimpleIOCContainer)
          , "SimpleIOCContainer.Docs.DiagnosticSchema.xml")]
        public string resource;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Resource = resource;
            return eo;
        }
    }
}