using System;
using System.Collections.Generic;
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

    [IOCCDependency]
    public class ClassTree
    {
        [IOCCInjectedDependency] private StructChild2 structChild2;
    }
    [IOCCDependency]
    public struct StructChild2
    {
        [IOCCInjectedDependency] private ClassChild classChild;
    }
    [IOCCDependency]
    public class ClassChild
    {
        [IOCCInjectedDependency] public int b = 1;
    }
    [IOCCDependency]
    public struct NoArgRoot
    {
    }
    [IOCCDependency]
    public class NoArgClassTree
    {
        [IOCCInjectedDependency] private NoArgStructChild2 structChild2;
    }
    [IOCCDependency]
    public struct NoArgStructChild2
    {
    }
}