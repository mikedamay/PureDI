using System.Security.AccessControl;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestCode
{
    [Bean]
    internal class CrossConnections
    {
        [BeanReference] private ChildA childA;
        [BeanReference] private ChildB childB;
        public ChildA ChildA => childA;
        public ChildB ChildB => childB;
    }

    [Bean]
    internal class ChildB
    {
        [BeanReference] private Common common;
        [BeanReference] private GrandChildB grandChildB;
        public Common Common => common;
        public GrandChildB GrandChildB => grandChildB;
    }
    [Bean]
    internal class GrandChildB
    {
        public string Name => "GrandChildB";
    }

    [Bean]
    internal class GrandChildA
    {
        public string Nme = "GrandChildA";
    }

    [Bean]
    internal class Common
    {
        public string Name = "Common";
    }

    [Bean]
    internal class ChildA
    {
        [BeanReference] private Common common;
        [BeanReference] private GrandChildA grandChildA;
        public Common Common => common;
        public GrandChildA GrandChildA => grandChildA;
    }
}