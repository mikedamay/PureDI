using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using SimpleIOCCDocumentor;

namespace PureDIDocumentor.Scene
{
    internal class Scene
    {
        public void Build()
        {
            var a = new Fact<IDocumentParser>
            {
                title = "mike"
                ,Children = new Facts
                {
                    new Fact<MySecond>
                    {
                        title = "bob"
                    }
                    , new Fact<string>
                    {
                        title = "alice"
                        ,value = "mike"
                    }
                } 
            };
        }

    }

    internal class Facts : List<object>
    {
        
    }
    
    
    internal class Fact<T>
    {
        public string title;
        public Facts Children;
        public T value;
    }
    
    internal class MyFirst {}
    internal class MySecond {}
    
}