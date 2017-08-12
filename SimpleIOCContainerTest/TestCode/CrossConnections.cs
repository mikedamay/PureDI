using System.Security.AccessControl;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [IOCCBean]
    internal class CrossConnections
    {
        [IOCCBeanReference] private ChildA childA;
        [IOCCBeanReference] private ChildB childB;
        public ChildA ChildA => childA;
        public ChildB ChildB => childB;
    }

    [IOCCBean]
    internal class ChildB
    {
        [IOCCBeanReference] private Common common;
        [IOCCBeanReference] private GrandChildB grandChildB;
        public Common Common => common;
        public GrandChildB GrandChildB => grandChildB;
    }
    [IOCCBean]
    internal class GrandChildB
    {
        public string Name => "GrandChildB";
    }

    [IOCCBean]
    internal class GrandChildA
    {
        public string Nme = "GrandChildA";
    }

    [IOCCBean]
    internal class Common
    {
        public string Name = "Common";
    }

    [IOCCBean]
    internal class ChildA
    {
        [IOCCBeanReference] private Common common;
        [IOCCBeanReference] private GrandChildA grandChildA;
        public Common Common => common;
        public GrandChildA GrandChildA => grandChildA;
    }
}