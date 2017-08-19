using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCBean(OS = SimpleIOCContainer.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [IOCCBean( OS = SimpleIOCContainer.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [IOCCBean( OS = SimpleIOCContainer.OS.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [IOCCBean(OS = SimpleIOCContainer.OS.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}