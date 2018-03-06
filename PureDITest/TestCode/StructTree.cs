using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.TestCode
{
    [Bean]
    public struct StructTree
    {
        [BeanReference]
        public StructChild structChild;
    }
    [Bean]
    public struct StructChild
    {
        [BeanReference]
        public int someValue;
    }

    public class RefResults
    {
        
    }

    [Bean]
    public class ClassTree
    {
        [BeanReference] private StructChild2 structChild2;

        public ref StructChild2 GetStructChild2()
        {
            return ref structChild2;
        }
    }
    [Bean]
    public struct StructChild2
    {
#pragma warning disable 649
        [BeanReference] private ClassChild _classChild;
#pragma warning restore 649

         public ClassChild GetClassChild()
        {
            return _classChild;
        }
    }
    [Bean]
    public class ClassChild
    {
        public int someValue = 1;
    }

    [Bean]
    public class NoArgRoot
    {
        public NoArgRoot(int itnowhasanarg) { }
    }
    [Bean]
    public class NoArgClassTree
    {
        #pragma warning disable 414
        [BeanReference] private NoArgStructChild2 structChild2 = new NoArgStructChild2();
    }
    [Bean]
    public struct NoArgStructChild2
    {
    }

    [Bean]
    public struct StructRoot
    {
        [BeanReference] public SomeChild child;
    }

    [Bean]
    public class SomeChild
    {
        
    }
}