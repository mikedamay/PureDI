using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    public struct StructTree
    {
        [IOCCBeanReference]
        public StructChild structChild;
    }
    [IOCCBean]
    public struct StructChild
    {
        [IOCCBeanReference]
        public int someValue;
    }

    public class RefResults
    {
        
    }

    [IOCCBean]
    public class ClassTree
    {
        [IOCCBeanReference] private StructChild2 structChild2;

        public ref StructChild2 GetStructChild2()
        {
            return ref structChild2;
        }
    }
    [IOCCBean]
    public struct StructChild2
    {
        [IOCCBeanReference] private ClassChild classChild;
        public ClassChild GetClassChild()
        {
            return classChild;
        }
    }
    [IOCCBean]
    public class ClassChild
    {
        public int someValue = 1;
    }

    [IOCCBean]
    public class NoArgRoot
    {
        public NoArgRoot(int itnowhasanarg) { }
    }
    [IOCCBean]
    public class NoArgClassTree
    {
        #pragma warning disable 414
        [IOCCBeanReference] private NoArgStructChild2 structChild2 = new NoArgStructChild2();
    }
    [IOCCBean]
    public struct NoArgStructChild2
    {
    }

    [IOCCBean]
    public struct StructRoot
    {
        [IOCCBeanReference] public SomeChild child;
    }

    [IOCCBean]
    public class SomeChild
    {
        
    }
}