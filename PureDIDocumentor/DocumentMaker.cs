using System;
using System.IO;
using PureDI;

namespace SimpleIOCCDocumentor
{
    internal interface IResourceProvider
    { 
        string GetResourceAsString(Type assemblyFinder, string path);
    }

    [Bean(Profile = "authoring")]
    internal class FileResourceProvider : IResourceProvider
    {
        public string GetResourceAsString(Type assemblyFinder, string path)
        {
            return File.ReadAllText(path);
        }
    }
    [Bean]
    internal class EmbeddedResourceProvider : ResourceFactoryBase, IResourceProvider
    {

    }
}