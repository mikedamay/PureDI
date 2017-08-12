using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCBean(OS = IOCC.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [IOCCBean( OS = IOCC.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [IOCCBean( OS = IOCC.OS.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [IOCCBean(OS = IOCC.OS.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [IOCCBean(OS = IOCC.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [IOCCBean(OS = IOCC.OS.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [IOCCBean(OS = IOCC.OS.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [IOCCBean(OS = IOCC.OS.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [IOCCBean(OS = IOCC.OS.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}