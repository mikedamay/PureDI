using PureDI;
using PureDI.Attributes;
using PureDI.Public;

namespace IOCCTest.TestData
{
    [Bean(OS = Os.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [Bean( OS = Os.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [Bean( OS = Os.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [Bean(OS = Os.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [Bean(OS = Os.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [Bean(OS = Os.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [Bean(OS = Os.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [Bean(OS = Os.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [Bean(OS = Os.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}