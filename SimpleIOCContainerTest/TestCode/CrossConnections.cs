using System.Security.AccessControl;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCDependency]
    internal class CrossConnections
    {
        [IOCCInjectedDependency] private ChildA childA;
        [IOCCInjectedDependency] private ChildB childB;
        public ChildA ChildA => childA;
        public ChildB ChildB => childB;
    }

    [IOCCDependency]
    internal class ChildB
    {
        [IOCCInjectedDependency] private Common common;
        [IOCCInjectedDependency] private GrandChildB grandChildB;
        public Common Common => common;
        public GrandChildB GrandChildB => grandChildB;
    }
    [IOCCDependency]
    internal class GrandChildB
    {
        public string Name => "GrandChildB";
    }

    [IOCCDependency]
    internal class GrandChildA
    {
        public string Nme = "GrandChildA";
    }

    [IOCCDependency]
    internal class Common
    {
        public string Name = "Common";
    }

    [IOCCDependency]
    internal class ChildA
    {
        [IOCCInjectedDependency] private Common common;
        [IOCCInjectedDependency] private GrandChildA grandChildA;
        public Common Common => common;
        public GrandChildA GrandChildA => grandChildA;
    }
}