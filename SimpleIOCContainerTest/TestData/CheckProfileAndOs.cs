using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCDependency(OS = IOCC.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [IOCCDependency( OS = IOCC.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [IOCCDependency( OS = IOCC.OS.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [IOCCDependency(OS = IOCC.OS.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [IOCCDependency(OS = IOCC.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [IOCCDependency(OS = IOCC.OS.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [IOCCDependency(OS = IOCC.OS.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [IOCCDependency(OS = IOCC.OS.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [IOCCDependency(OS = IOCC.OS.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}