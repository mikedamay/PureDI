using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [Bean(OS = SimpleIOCContainer.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [Bean( OS = SimpleIOCContainer.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [Bean( OS = SimpleIOCContainer.OS.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [Bean(OS = SimpleIOCContainer.OS.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}