using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    public struct StructTree
    {
        [IOCCInjectedDependency]
        public StructChild structChild;
    }
    [IOCCDependency]
    public struct StructChild
    {
        [IOCCInjectedDependency]
        public int someValue;
    }

    public class RefResults
    {
        
    }

    [IOCCDependency]
    public class ClassTree
    {
        [IOCCInjectedDependency] private StructChild2 structChild2;

        public ref StructChild2 GetStructChild2()
        {
            return ref structChild2;
        }
    }
    [IOCCDependency]
    public struct StructChild2
    {
        [IOCCInjectedDependency] private ClassChild classChild;
        public ClassChild GetClassChild()
        {
            return classChild;
        }
    }
    [IOCCDependency]
    public class ClassChild
    {
        public int someValue = 1;
    }

    [IOCCDependency]
    public class NoArgRoot
    {
        public NoArgRoot(int itnowhasanarg) { }
    }
    [IOCCDependency]
    public class NoArgClassTree
    {
        #pragma warning disable 414
        [IOCCInjectedDependency] private NoArgStructChild2 structChild2 = new NoArgStructChild2();
    }
    [IOCCDependency]
    public struct NoArgStructChild2
    {
    }

    [IOCCDependency]
    public struct StructRoot
    {
        [IOCCInjectedDependency] public SomeChild child;
    }

    [IOCCDependency]
    public class SomeChild
    {
        
    }
}