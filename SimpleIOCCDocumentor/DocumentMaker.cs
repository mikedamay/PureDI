using System;
using System.IO;
using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDocumentor
{
    internal interface IDocumentMaker
    { 
        string GetResourceAsString(Type assemblyFinder, string path);
    }

    [Bean(Profile = "authoring")]
    internal class FileDocumentMaker : IDocumentMaker
    {
        public string GetResourceAsString(Type assemblyFinder, string path)
        {
            return File.ReadAllText(path);
        }
    }
    [Bean]
    internal class ResourceDocumentMaker : ResourceFactoryBase, IDocumentMaker
    {

    }
}