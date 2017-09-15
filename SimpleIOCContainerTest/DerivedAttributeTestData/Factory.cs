using System;
using System.Dynamic;
using System.Reflection;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
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